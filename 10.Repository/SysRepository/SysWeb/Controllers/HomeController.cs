using SysApplication.UserService;
using SysEntity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SysWeb.Controllers
{
    public class HomeController : Controller
    {
        public readonly IUserService _userService;
        public HomeController(IUserService userService)
        {
            _userService = userService;
        }

        public ActionResult Index()
        {
            var entity = new UserEntity()
            {
                Id = Guid.NewGuid(),
                UserName = DateTime.Now.ToString()
            };
            _userService.Add(entity);
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}