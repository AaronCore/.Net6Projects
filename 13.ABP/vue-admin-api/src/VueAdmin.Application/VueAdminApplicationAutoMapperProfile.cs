using AutoMapper;
using VueAdmin.Domain.System;
using VueAdmin.Application.Contracts.System.Account;
using VueAdmin.Application.Contracts.System.Menu;
using VueAdmin.Application.Contracts.System.Role;

namespace VueAdmin.Application
{
    public class VueAdminApplicationAutoMapperProfile : Profile
    {
        public VueAdminApplicationAutoMapperProfile()
        {
            // Menu
            CreateMap<MenuInput, MenuEntity>()
                .ForMember(x => x.Id, opt => opt.Ignore())
                .ForMember(x => x.Creater, opt => opt.Ignore())
                .ForMember(x => x.CreateTime, opt => opt.Ignore())
                .ForMember(x => x.Editor, opt => opt.Ignore())
                .ForMember(x => x.EditTime, opt => opt.Ignore());
            CreateMap<MenuEntity, MenuOut>()
                .ForMember(x => x.Id, opt => opt.MapFrom(o => o.Id.ToString()))
                .ForMember(x => x.Creater, opt => opt.Ignore())
                .ForMember(x => x.CreateTime, opt => opt.MapFrom(o => o.CreateTime.ToString("yyyy-MM-dd HH:mm:ss")))
                .ForMember(x => x.Editor, opt => opt.Ignore())
                .ForMember(x => x.EditTime, opt => opt.Ignore());

            // Role
            CreateMap<RoleInput, RoleEntity>()
                .ForMember(x => x.Id, opt => opt.Ignore())
                .ForMember(x => x.Creater, opt => opt.Ignore())
                .ForMember(x => x.CreateTime, opt => opt.Ignore())
                .ForMember(x => x.Editor, opt => opt.Ignore())
                .ForMember(x => x.EditTime, opt => opt.Ignore());
            CreateMap<RoleEntity, RoleOut>()
                .ForMember(x => x.Id, opt => opt.MapFrom(o => o.Id.ToString()))
                .ForMember(x => x.Creater, opt => opt.Ignore())
                .ForMember(x => x.CreateTime, opt => opt.MapFrom(o => o.CreateTime.ToString("yyyy-MM-dd HH:mm:ss")))
                .ForMember(x => x.Editor, opt => opt.Ignore())
                .ForMember(x => x.EditTime, opt => opt.Ignore());

            //Account
            CreateMap<AccountInput, AccountEntity>()
                .ForMember(x => x.Id, opt => opt.Ignore())
                .ForMember(x => x.Creater, opt => opt.Ignore())
                .ForMember(x => x.CreateTime, opt => opt.Ignore())
                .ForMember(x => x.Editor, opt => opt.Ignore())
                .ForMember(x => x.EditTime, opt => opt.Ignore());
            CreateMap<AccountEntity, AccountOut>()
                .ForMember(x => x.Id, opt => opt.MapFrom(o => o.Id.ToString()))
                .ForMember(x => x.Creater, opt => opt.Ignore())
                .ForMember(x => x.CreateTime, opt => opt.MapFrom(o => o.CreateTime.ToString("yyyy-MM-dd HH:mm:ss")))
                .ForMember(x => x.Editor, opt => opt.Ignore())
                .ForMember(x => x.EditTime, opt => opt.Ignore());
        }
    }
}
