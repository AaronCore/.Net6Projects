using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;

namespace AutoMapperApp
{
    public class OrganizationProfile : Profile
    {
        public OrganizationProfile()
        {
            CreateMap<UserEntity, UserDto>()
                .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.Name))
                .ReverseMap()
                ;
        }
    }
}
