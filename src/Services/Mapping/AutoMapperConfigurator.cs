using ApplicationCore.Entities;
using AutoMapper;
using Infrastructure.Identity;
using Services.ViewModels;
using System.Linq;

namespace Services.Mapping
{
    public class AutoMapperConfigurator : Profile
    {
        public AutoMapperConfigurator()
        {
            CreateMap<User, UserViewModel>().ForMember(dest => dest.Token, opt => opt.MapFrom(src => src.RefreshTokens.Any(s => !s.IsExpired) ?
            src.RefreshTokens.First(s => !s.IsExpired).Token : string.Empty));
            CreateMap<ApplicationUser, User>().ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email)).
                                       ForMember(dest => dest.PasswordHash, opt => opt.MapFrom(src => src.PasswordHash)).
                                       ForAllOtherMembers(opt => opt.Ignore());
            CreateMap<User, ApplicationUser>().ConstructUsing(u => new ApplicationUser { UserName = u.UserName, Email = u.Email })
                .ForMember(au => au.Id, opt => opt.Ignore())
                .ForMember(au => au.PasswordHash, u => u.MapFrom(src => src.PasswordHash))
                .ForAllOtherMembers(opt => opt.Ignore());
        }
    }
}
