using AutoMapper;
using EasyStore.Application.User.Dto;
using EasyStore.Domain;

namespace EasyStore.Application
{
    public class EasyStoreApplicationAutoMapperProfile : Profile
    {
        public EasyStoreApplicationAutoMapperProfile()
        {
            CreateMap<Users, UserDto>().ReverseMap();
        }
    }
}
