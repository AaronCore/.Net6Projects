using AutoMapper;
using EFCoreWebAPI.Sample.DtoModels;
using EFCoreWebAPI.Sample.Entitys;

namespace EFCoreWebAPI.Sample.Profiles
{
    public class StudentProfile : Profile
    {
        public StudentProfile()
        {
            CreateMap<Students, StudentDto>()
                .ForMember(dest => dest.StudentId, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.StudentNo, opt => opt.MapFrom(src => src.No))
                .ForMember(dest => dest.StudentName, opt => opt.MapFrom(src => src.Name))
                ;

            CreateMap<AddOrUpdateStudentDto, Students>()
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.StudentName))
                .ForMember(dest => dest.No, opt => opt.MapFrom(src => src.StudentNo))
                ;
            CreateMap<StudentCourseDto, StudentCourses>();
        }
    }
}
