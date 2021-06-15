using AutoMapper;
using WebApp.Models;

namespace WebApp
{
    public class AutoMapperConfig
    {
        public IMapper Register()
        {
            var configuration = new MapperConfiguration(c =>
            {
                c.AddProfile(new EnTypeMapper());
            });
            configuration.AssertConfigurationIsValid();
            return new Mapper(configuration);
        }
    }
    public class EnTypeMapper : Profile, IProfile
    {
        public EnTypeMapper()
        {
            CreateMap<UserEntity, UserDto>()
                .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.Name))
                .ReverseMap()
                ;
        }
    }
    public interface IProfile
    {

    }
}