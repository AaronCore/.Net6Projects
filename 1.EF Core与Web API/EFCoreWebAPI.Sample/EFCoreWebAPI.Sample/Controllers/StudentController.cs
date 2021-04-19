using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using EFCoreWebAPI.Sample.DataContext;
using EFCoreWebAPI.Sample.DtoModels;
using EFCoreWebAPI.Sample.Entitys;

namespace EFCoreWebAPI.Sample.Controllers
{
    /// <summary>
    /// 学生
    /// </summary>
    [Route("api/student")]
    [ApiController]
    public class StudentController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly DataDbContext _context;

        public StudentController(IMapper mapper, DataDbContext context)
        {
            _mapper = mapper;
            _context = context ?? throw new ArgumentNullException(nameof(context)); ;
        }

        /// <summary>
        /// 学生集合
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetStudents()
        {
            var studentList = await _context.Students.ToListAsync();

            if (!studentList.Any())
            {
                return NotFound();
            }

            var studentDtos = _mapper.Map<IEnumerable<StudentDto>>(studentList);
            return Ok(studentDtos);
        }

        /// <summary>
        /// 获取学生信息
        /// </summary>
        /// <param name="studentId">学生Id</param>
        /// <returns></returns>
        [HttpGet("{studentId}")]
        public async Task<IActionResult> GetStudent(Guid studentId)
        {
            var studentEntity = await _context.Students.FirstOrDefaultAsync(p => p.Id == studentId);

            if (studentEntity == null)
            {
                return NotFound();
            }
            var studentDto = _mapper.Map<StudentDto>(studentEntity);

            var returnObj = new
            {
                studentDto,
                StudentCourses = studentEntity.StudentCourses?.Select(p => new CourseDto()
                {
                    CourseId = p.Course.Id,
                    CourseName = p.Course.Name
                }).ToList()
            };
            return Ok(returnObj);
        }

        /// <summary>
        /// 添加学生
        /// </summary>
        /// <param name="addStudent">学生信息</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> CreateStudent(AddOrUpdateStudentDto addStudent)
        {
            if (addStudent == null)
            {
                return NotFound();
            }

            //var classEntity = await _context.Class.FirstOrDefaultAsync(p => p.Id == addStudent.ClassId);
            //if (classEntity == null)
            //{
            //    return NotFound();
            //}

            var studentEntity = _mapper.Map<Students>(addStudent);
            studentEntity.Id = Guid.NewGuid();
            studentEntity.Gender = Enum.Parse<Gender>(addStudent.Gender);

            studentEntity.ClassId = addStudent.ClassId;
            //studentEntity.Class = classEntity;

            studentEntity.ArchiveId = Guid.NewGuid();
            studentEntity.Archive = new Archives()
            {
                Id = studentEntity.ArchiveId,
                StudentId = studentEntity.Id,
                No = DateTime.Now.ToString("yyyMMddHHmmsss"),
                CreateTime = DateTime.Now,
            };

            if (addStudent.StudentCourseIds.Any())
            {
                var studentCourseIds = addStudent.StudentCourseIds;
                var courses = await _context.Courses.Where(p => studentCourseIds.Contains(p.Id)).ToListAsync();
                var studentCourseDtos = courses.Select(p => new StudentCourseDto()
                {
                    StudentId = studentEntity.Id,
                    CourseId = p.Id
                });
                var studentCourses = _mapper.Map<IEnumerable<StudentCourses>>(studentCourseDtos);
                studentEntity.StudentCourses = studentCourses.ToList();
            }

            _context.Students.Add(studentEntity);
            await _context.SaveChangesAsync();

            return Ok();
        }

        /// <summary>
        /// 修改学生信息
        /// </summary>
        /// <param name="studentId">学生Id</param>
        /// <param name="updateStudent">学生信息</param>
        /// <returns></returns>
        [HttpPut]
        public async Task<IActionResult> UpdateStudent(Guid studentId, AddOrUpdateStudentDto updateStudent)
        {
            if (updateStudent == null)
            {
                return NotFound();
            }

            //var classEntity = await _context.Class.FirstOrDefaultAsync(p => p.Id == updateStudent.ClassId);
            //if (classEntity == null)
            //{
            //    return NotFound();
            //}

            var studentEntity = await _context.Students.FirstOrDefaultAsync(p => p.Id == studentId);
            _mapper.Map(updateStudent, studentEntity);
            studentEntity.Gender = Enum.Parse<Gender>(updateStudent.Gender);

            studentEntity.ClassId = updateStudent.ClassId;
            //studentEntity.Class = classEntity;

            if (updateStudent.StudentCourseIds.Any())
            {
                var studentCourseIds = updateStudent.StudentCourseIds;
                var courses = await _context.Courses.Where(p => studentCourseIds.Contains(p.Id)).ToListAsync();
                var studentCourseDtos = courses.Select(p => new StudentCourseDto()
                {
                    StudentId = studentEntity.Id,
                    CourseId = p.Id
                });
                var studentCourses = _mapper.Map<IEnumerable<StudentCourses>>(studentCourseDtos);
                studentEntity.StudentCourses.Clear();
                studentEntity.StudentCourses = studentCourses.ToList();
            }

            _context.Students.Update(studentEntity);
            await _context.SaveChangesAsync();

            return Ok();
        }

        /// <summary>
        /// 删除学生
        /// </summary>
        /// <param name="studentId">学生Id</param>
        /// <returns></returns>
        [HttpDelete("{studentId}")]
        public async Task<IActionResult> DelStudent(Guid studentId)
        {
            var studnetEntity = await _context.Students.FirstOrDefaultAsync(p => p.Id == studentId);
            if (studnetEntity == null)
            {
                return NotFound();
            }
            _context.Students.Remove(studnetEntity);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
