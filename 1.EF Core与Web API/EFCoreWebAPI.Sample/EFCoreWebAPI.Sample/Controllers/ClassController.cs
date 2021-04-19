using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EFCoreWebAPI.Sample.DataContext;
using EFCoreWebAPI.Sample.DtoModels;
using EFCoreWebAPI.Sample.Entitys;

namespace EFCoreWebAPI.Sample.Controllers
{
    /// <summary>
    /// 班级
    /// </summary>
    [Route("api/class")]
    [ApiController]
    public class ClassController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly DataDbContext _context;

        public ClassController(IMapper mapper, DataDbContext context)
        {
            _mapper = mapper;
            _context = context ?? throw new ArgumentNullException(nameof(context)); ;
        }

        /// <summary>
        /// 班级集合
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetClass()
        {
            var classList = await _context.Class.ToListAsync();

            if (!classList.Any())
            {
                return NotFound();
            }

            var classDtos = _mapper.Map<IEnumerable<ClassDto>>(classList);
            return Ok(classDtos);
        }

        /// <summary>
        /// 班级查询
        /// </summary>
        /// <param name="className">班级名称</param>
        /// <returns></returns>
        [HttpGet("{className}")]
        public async Task<IActionResult> GetClass(string className)
        {
            var classList = await _context.Class.Where(p => p.Name.Contains(className))
                .OrderByDescending(p => p.Name).ToListAsync();

            if (!classList.Any())
            {
                return NotFound();
            }

            var classDtos = _mapper.Map<IEnumerable<ClassDto>>(classList);
            return Ok(classDtos);
        }

        /// <summary>
        /// 添加班级
        /// </summary>
        /// <param name="addClass">班级信息</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> CreateClass(AddOrUpdateClassDto addClass)
        {
            var classEntity = _mapper.Map<Class>(addClass);
            classEntity.Id = Guid.NewGuid();

            _context.Class.Add(classEntity);
            await _context.SaveChangesAsync();

            return Ok();
        }

        /// <summary>
        /// 班级删除
        /// </summary>
        /// <param name="classId">班级Id</param>
        /// <returns></returns>
        [HttpDelete("{classId}")]
        public async Task<IActionResult> DelClass(Guid classId)
        {
            var classEntity = await _context.Class.FirstOrDefaultAsync(p => p.Id == classId);
            if (classEntity == null)
            {
                return NotFound();
            }

            _context.Class.Remove(classEntity);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        /// <summary>
        /// 班级修改
        /// </summary>
        /// <param name="classId">班级Id</param>
        /// <param name="updateClass">班级信息</param>
        /// <returns></returns>
        [HttpPut]
        public async Task<IActionResult> UpdateClass(Guid classId, AddOrUpdateClassDto updateClass)
        {
            var classEntity = await _context.Class.FirstOrDefaultAsync(p => p.Id == classId);
            if (classEntity == null)
            {
                return NotFound();
            }
            _mapper.Map(updateClass, classEntity);

            _context.Class.Update(classEntity);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
