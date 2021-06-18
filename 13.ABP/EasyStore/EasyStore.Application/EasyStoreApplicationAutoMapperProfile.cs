using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using EasyStore.Application.User.Dto;
using EasyStore.Domain;

namespace EasyStore.Application
{
    public class EasyStoreApplicationAutoMapperProfile : Profile
    {
        public EasyStoreApplicationAutoMapperProfile()
        {
            CreateMap<Users, UserDto>()
                .ReverseMap();
        }
    }
}
