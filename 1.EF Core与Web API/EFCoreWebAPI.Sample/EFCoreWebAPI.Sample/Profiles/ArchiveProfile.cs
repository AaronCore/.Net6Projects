using AutoMapper;
using EFCoreWebAPI.Sample.DtoModels;
using EFCoreWebAPI.Sample.Entitys;

namespace EFCoreWebAPI.Sample.Profiles
{
    public class ArchiveProfile : Profile
    {
        public ArchiveProfile()
        {
            CreateMap<Archives, ArchiveDto>()
                .ForMember(dest => dest.ArchiveId, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.ArchiveNo, opt => opt.MapFrom(src => src.No))
                ;
        }
    }
}
