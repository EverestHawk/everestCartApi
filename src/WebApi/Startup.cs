using System;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using ApplicationCore;
using ApplicationCore.Interfaces;
using ApplicationCore.Services;
using ApplicationCore.TypeMapping;
using AutoMapper;
using Infrastructure.Auth;
using Infrastructure.Data;
using Infrastructure.Identity;
using Infrastructure.Logging;
using Infrastructure.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Services.Implementations;
using Services.Interfaces;
using WebApi.Interfaces;
using WebApi.Services;
using static Infrastructure.Helpers.Constants;

namespace WebApi
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureDevelopmentServices(IServiceCollection services)
        {
            //ConfigureTestingServices(services);
            //using real database
            ConfigureProductionServices(services);
        }

        public void ConfigureTestingServices(IServiceCollection services)
        {
            // use in-memory database
            services.AddDbContext<CatalogContext>(c =>
                c.UseInMemoryDatabase("Catalog"));

            // Add Identity DbContext
            services.AddDbContext<AppIdentityDbContext>(options =>
                options.UseInMemoryDatabase("Identity"));

            ConfigureServices(services);
        }

        public void ConfigureProductionServices(IServiceCollection services)
        {
            //Requires LocalDB, which can be installed with SQL server Express 2016
            services.AddDbContext<CatalogContext>(c =>
            c.UseSqlServer(Configuration.GetConnectionString("CatalogConnection"), b => b.MigrationsAssembly("Infrastructure")));

            //Identity DbContext
            services.AddDbContext<AppIdentityDbContext>(i =>
            i.UseSqlServer(Configuration.GetConnectionString("IdentityConnection"), a => a.MigrationsAssembly("Infrastructure")));

            ConfigureServices(services);
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<AppIdentityDbContext>()
                .AddDefaultTokenProviders();            

            services.AddScoped(typeof(IRepository<>), typeof(EfRepository<>));
            services.AddScoped(typeof(IAsyncRepository<>), typeof(EfRepository<>));

            services.AddScoped<IBasketService, BasketService>();
            services.AddScoped<IOrderService, OrderService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IManageService, ManageService>();
            services.AddScoped<ICatalogService, CachedCatalogService>();
            services.AddScoped<CatalogService>();
            services.AddScoped<IJwtFactory, JwtFactory>();
            services.AddScoped<IJwtTokenHandler, JwtTokenHandler>();
            services.AddScoped<IJwtTokenValidator, JwtTokenValidator>();
            services.AddScoped<ITokenFactory, TokenFactory>();
            services.AddSingleton<IUriComposer>(new UriComposer(Configuration.Get<CatalogSettings>()));
            services.AddScoped(typeof(IAppLogger<>), typeof(LoggerAdapter<>));
            services.AddTransient<IEmailSender, EmailSender>();
            services.AddScoped<IAutoMapper, AutoMapperAdapter>();

            services.Configure<CatalogSettings>(Configuration);

            var authSettings = Configuration.GetSection(nameof(AuthSettings));
            services.Configure<AuthSettings>(authSettings);

            var signingKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(authSettings[nameof(AuthSettings.SecretKey)]));

            //jwt wire up
            //Get options from app settings
            var jwtIssuerOptionsSettings = Configuration.GetSection(nameof(JwtIssuerOptions));
            services.Configure<JwtIssuerOptions>(options =>
            {
                options.Issuer = jwtIssuerOptionsSettings[nameof(JwtIssuerOptions.Issuer)];
                options.Audience = jwtIssuerOptionsSettings[nameof(JwtIssuerOptions.Audience)];
                options.SigningCredentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);
            });

            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidIssuer = jwtIssuerOptionsSettings[nameof(JwtIssuerOptions.Issuer)],

                ValidateAudience = true,
                ValidAudience = jwtIssuerOptionsSettings[nameof(JwtIssuerOptions.Audience)],

                ValidateIssuerSigningKey = true,
                IssuerSigningKey = signingKey,

                RequireExpirationTime = false,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero
            };

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(configureOptions =>
            {
                configureOptions.ClaimsIssuer = jwtIssuerOptionsSettings[nameof(JwtIssuerOptions.Issuer)];
                configureOptions.TokenValidationParameters = tokenValidationParameters;
                configureOptions.SaveToken = true;

                configureOptions.Events = new JwtBearerEvents
                {
                    OnAuthenticationFailed = context =>
                    {
                        if (context.Exception.GetType() == typeof(SecurityTokenExpiredException))
                        {
                            context.Response.Headers.Add("Token-Expired", "true");
                        }
                        return Task.CompletedTask;
                    }
                };
            });

            //Api user claim policy
            services.AddAuthorization(options =>
            {
                options.AddPolicy("ApiUser", policy => policy.RequireClaim(JwtClaimIdentifiers.Role, JwtClaims.ApiAccess));
            });

            //add identity password requirement
            var identityBuilder = services.AddIdentityCore<ApplicationUser>(o =>
            {
                // configure identity options
                o.Password.RequireDigit = false;
                o.Password.RequireLowercase = false;
                o.Password.RequireUppercase = false;
                o.Password.RequireNonAlphanumeric = false;
                o.Password.RequiredLength = 6;
            });

            identityBuilder = new IdentityBuilder(identityBuilder.UserType, typeof(IdentityRole), identityBuilder.Services);
            identityBuilder.AddEntityFrameworkStores<AppIdentityDbContext>().AddDefaultTokenProviders();

            // Add memory cache services
            services.AddMemoryCache();
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
            services.AddCors(options =>
            {
                options.AddPolicy("ecCorsPolicy",
                    builder =>
                    {
                        builder.AllowAnyOrigin()
                        .AllowAnyHeader()
                        .AllowAnyMethod()
                        .AllowCredentials();
                    });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.UseCors("ecCorsPolicy");

            app.UseAuthentication();

            app.UseMvc();
        }
    }
}
