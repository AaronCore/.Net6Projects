using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using EasyStore.Application.User;
using EasyStore.Application.User.Dto;

namespace EasyStore.Web.Controllers
{
    public class UserController : Controller
    {
        private readonly IUserService _userService;
        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> AddUser(string name, string address)
        {
            var userDto = new UserDto()
            {
                Name = name,
                Address = address,
                CreateTime = DateTime.Now
            };
            var user = await _userService.AddUser(userDto);
            return Json(user);
        }

        [HttpGet]
        public async Task<IActionResult> GetUser(string name)
        {
            var user = await _userService.GetUser("jack");
            return Json(user);
        }

        [HttpGet]
        public async Task<IActionResult> GetUsers()
        {
            var users = await _userService.GetUsers();
            return Json(users);
        }

        [HttpGet]
        public async Task<IActionResult> UpdateUser(int id, string address)
        {
            var updateUser = await _userService.UpdateUser(id, address);
            return Json(updateUser);
        }

        [HttpGet]
        public async Task<IActionResult> DeleteUser(int id)
        {
            await _userService.DeleteUser(id);
            return Ok();
        }
    }
}
