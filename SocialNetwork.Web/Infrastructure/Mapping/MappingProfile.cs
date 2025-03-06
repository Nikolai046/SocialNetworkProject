using AutoMapper;
using SocialNetwork.Core.Models.ViewModels.Account;
using SocialNetwork.Core.Models.ViewModels.DTO;
using SocialNetwork.Data.Entities;


namespace SocialNetwork.Web.Infrastructure.Mapping;

/// <summary>
/// Класс MappingProfile определяет правила маппинга между различными моделями данных, используя библиотеку AutoMapper.
/// Он содержит три маппинга: 
/// 1. Из RegisterViewModel в User, где поля BirthDate, Email и UserName заполняются на основе соответствующих полей из RegisterViewModel.
/// 2. Из User в UpdateViewModel, где поля FirstName, LastName, Email, Login, Image, Year, Month, Date и Status заполняются на основе соответствующих полей из User.
/// 3. Из UpdateViewModel в User, где поля FirstName, LastName, Email, UserName, Image, Status, BirthDate и About заполняются на основе соответствующих полей из UpdateViewModel.
/// </summary>
public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<RegisterViewModel, User>()
            .ForMember(x => x.BirthDate, opt => opt.MapFrom(c => new DateTime((int)c.Year, (int)c.Month, (int)c.Date)))
            .ForMember(x => x.Email, opt => opt.MapFrom(c => c.EmailReg))
            .ForMember(x => x.UserName, opt => opt.MapFrom(c => c.Login));
        CreateMap<User, UpdateViewModel>()
            .ForMember(x => x.FirstName, opt => opt.MapFrom(c => c.FirstName))
            .ForMember(x => x.LastName, opt => opt.MapFrom(c => c.LastName))
            .ForMember(x => x.Email, opt => opt.MapFrom(c => c.Email))
            .ForMember(x => x.Login, opt => opt.MapFrom(c => c.UserName))
            .ForMember(x => x.Image, opt => opt.MapFrom(c => c.Image))
            .ForMember(x => x.Year, opt => opt.MapFrom(c => c.BirthDate.Year))
            .ForMember(x => x.Month, opt => opt.MapFrom(c => c.BirthDate.Month))
            .ForMember(x => x.Date, opt => opt.MapFrom(c => c.BirthDate.Day))
            .ForMember(x => x.Status, opt => opt.MapFrom(c => c.Status));
        CreateMap<UpdateViewModel, User>()
            .ForMember(x => x.FirstName, opt => opt.MapFrom(c => c.FirstName))
            .ForMember(x => x.LastName, opt => opt.MapFrom(c => c.LastName))
            .ForMember(x => x.Email, opt => opt.MapFrom(c => c.Email))
            .ForMember(x => x.UserName, opt => opt.MapFrom(c => c.Login))
            .ForMember(x => x.Image, opt => opt.MapFrom(c => c.Image))
            .ForMember(x => x.Status, opt => opt.MapFrom(c => c.Status))
            .ForMember(x => x.BirthDate, opt => opt.MapFrom(c => new DateTime((int)c.Year, (int)c.Month, (int)c.Date)))
            .ForMember(x => x.About, opt => opt.MapFrom(c => c.About));
        CreateMap<User, UserViewModel>()
            .ForMember(x => x.UserId, opt => opt.MapFrom(c => c.Id))
            .ForMember(x => x.FirstName, opt => opt.MapFrom(c => c.FirstName))
            .ForMember(x => x.LastName, opt => opt.MapFrom(c => c.LastName))
            .ForMember(x => x.FullName, opt => opt.MapFrom(c => string.Concat(c.FirstName, " ", c.LastName)))
            .ForMember(x => x.Email, opt => opt.MapFrom(c => c.Email))
            .ForMember(x => x.Image, opt => opt.MapFrom(c => c.Image))
            .ForMember(x => x.BirthDate, opt => opt.MapFrom(c => c.BirthDate))
            .ForMember(x => x.Status, opt => opt.MapFrom(c => c.Status))
            .ForMember(x => x.About, opt => opt.MapFrom(c => c.About));
        CreateMap<User, UserlistDto>()
            .ForMember(x => x.UserId, opt => opt.MapFrom(c => c.Id))
            .ForMember(x => x.FirstName, opt => opt.MapFrom(c => c.FirstName))
            .ForMember(x => x.LastName, opt => opt.MapFrom(c => c.LastName))
            .ForMember(x => x.FullName, opt => opt.MapFrom(c => string.Concat(c.FirstName, " ", c.LastName)))
            .ForMember(x => x.Email, opt => opt.MapFrom(c => c.Email))
            .ForMember(x => x.Image, opt => opt.MapFrom(c => c.Image))
            .ForMember(x => x.BirthDate, opt => opt.MapFrom(c => c.BirthDate))
            .ForMember(x => x.Status, opt => opt.MapFrom(c => c.Status))
            .ForMember(x => x.About, opt => opt.MapFrom(c => c.About))
            .ForMember(x => x.IsMyFriend, opt => opt.MapFrom(_ => false));
    }
}