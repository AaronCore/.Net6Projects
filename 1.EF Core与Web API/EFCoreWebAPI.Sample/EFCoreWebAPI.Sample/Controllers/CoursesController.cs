using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using EFCoreWebAPI.Sample.Commons;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using EFCoreWebAPI.Sample.DataContext;
using EFCoreWebAPI.Sample.DtoModels;

namespace EFCoreWebAPI.Sample.Controllers
{
    /// <summary>
    /// 课程
    /// </summary>
    [Route("api/courses")]
    [ApiController]
    public class CoursesController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly DataDbContext _context;

        public CoursesController(IMapper mapper, DataDbContext context)
        {
            _mapper = mapper;
            _context = context ?? throw new ArgumentNullException(nameof(context)); ;
        }

        /// <summary>
        /// 课程集合
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetCourses()
        {
            var list = await _context.Courses.ToListAsync();

            if (!list.Any())
            {
                return NotFound();
            }

            var dtos = _mapper.Map<IEnumerable<CourseDto>>(list);
            return Ok(dtos);
        }

        /// <summary>
        /// 课程查询
        /// </summary>
        /// <param name="ids">课程Id集合</param>
        /// <returns></returns>
        [HttpGet("{ids}")]
        public async Task<IActionResult> GetCourses(
            [FromRoute]
            [ModelBinder(BinderType = typeof(ArrayModelBinder))]
            IEnumerable<Guid> ids)
        {
            if (ids == null)
            {
                return BadRequest();
            }

            var list = await _context.Courses.Where(p => ids.Contains(p.Id)).ToListAsync();

            if (!list.Any())
            {
                return NotFound();
            }

            var dtos = _mapper.Map<IEnumerable<CourseDto>>(list);
            return Ok(dtos);
        }
    }
}
