using AutoMapper;
using SocialNetwork.DLL.Entities;
using SocialNetwork.Models.ViewModels.Account;

namespace SocialNetwork;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<RegisterViewModel, User>()
            .ForMember(x => x.BirthDate, opt => opt.MapFrom(c => new DateTime((int)c.Year, (int)c.Month, (int)c.Date)))
            .ForMember(x => x.Email, opt => opt.MapFrom(c => c.EmailReg))
            .ForMember(x => x.UserName, opt => opt.MapFrom(c => c.Login));
        //CreateMap<LoginViewModel, User>()
        //    .ForMember(x=>x.Email, opt=>opt.MapFrom(c=>c.Email))
        //    .ForMember(x => x.Pa, opt => opt.MapFrom(c => c.Email));

    }
}