using System;
using ApplicationCore.Entities;
using ApplicationCore.Entities.BasketAggregate;
using ApplicationCore.Entities.OrderAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data
{
    public class CatalogContext : DbContext
    {
        public CatalogContext(DbContextOptions<CatalogContext> options) : base(options)
        {
        }

        public DbSet<Basket> Baskets { get; set; }
        public DbSet<CatalogItem> CatalogItems { get; set; }
        public DbSet<CatalogBrand> CatalogBrands { get; set; }
        public DbSet<CatalogType> CatalogTypes { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<Basket>(ConfigureBasket);
            builder.Entity<CatalogBrand>(ConfigureCatalogBrand);
            builder.Entity<CatalogType>(ConfigureCatalogType);
            builder.Entity<CatalogItem>(ConfigureCatalog);
            builder.Entity<Order>(ConfigureOrder);
            builder.Entity<OrderItem>(ConfigureOrderItem);
            builder.Entity<User>(ConfigureUser);
            builder.Entity<RefreshToken>(ConfigureRefreshTokens);
        }

        private void ConfigureRefreshTokens(EntityTypeBuilder<RefreshToken> builder)
        {
            builder.HasKey(t => new { t.UserId, t.Token });
        }

        private void ConfigureUser(EntityTypeBuilder<User> builder)
        {
            var navigation = builder.Metadata.FindNavigation(nameof(User.RefreshTokens));
            //EF access the refreshtokens collection property through its backing field
            navigation.SetPropertyAccessMode(PropertyAccessMode.Field);

            builder.Ignore(u => u.Email);
            builder.Ignore(u => u.PasswordHash);
        }

        private void ConfigureCatalogType(EntityTypeBuilder<CatalogType> builder)
        {
            builder.ToTable("CatalogType");

            builder.HasKey(ct => ct.Id);

            builder.Property(ct => ct.Id)
                .ForSqlServerUseSequenceHiLo("catalog_type_hilo")
                .IsRequired();

            builder.Property(ct => ct.Type)
                .IsRequired()
                .HasMaxLength(100);
        }

        private void ConfigureCatalogBrand(EntityTypeBuilder<CatalogBrand> builder)
        {
            builder.ToTable("CatalogBrand");

            builder.HasKey(ct => ct.Id);

            builder.Property(ct => ct.Id)
                .ForSqlServerUseSequenceHiLo("catalog_brand_hilo")
                .IsRequired();

            builder.Property(ct => ct.Brand)
                .IsRequired()
                .HasMaxLength(100);
        }

        private void ConfigureBasket(EntityTypeBuilder<Basket> builder)
        {
            var navigation = builder.Metadata.FindNavigation(nameof(Basket.Items));

            navigation.SetPropertyAccessMode(PropertyAccessMode.Field);
        }

        private void ConfigureCatalog(EntityTypeBuilder<CatalogItem> builder)
        {
            builder.ToTable("Catalog");

            builder.Property(c => c.Id)
                .ForSqlServerUseSequenceHiLo("catalog_hilo")
                .IsRequired();

            builder.Property(c => c.Name)
                .IsRequired(true)
                .HasMaxLength(50);

            builder.Property(c => c.Price)
                .IsRequired(true);

            builder.Property(c => c.PictureUri)
                .IsRequired(false);

            builder.HasOne(c => c.CatalogBrand)
                .WithMany()
                .HasForeignKey(c => c.CatalogBrandId);

            builder.HasOne(c => c.CatalogType)
                .WithMany()
                .HasForeignKey(c => c.CatalogTypeId);
        }
        
        private void ConfigureOrder(EntityTypeBuilder<Order> builder)
        {
            var navigation = builder.Metadata.FindNavigation(nameof(Order.OrderItems));

            navigation.SetPropertyAccessMode(PropertyAccessMode.Field);

            builder.OwnsOne(o => o.ShipToAddress);
        }

        private void ConfigureOrderItem(EntityTypeBuilder<OrderItem> builder)
        {
            builder.OwnsOne(o => o.ItemOrdered);
        }
    }
}
