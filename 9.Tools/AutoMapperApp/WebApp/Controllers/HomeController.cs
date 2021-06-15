using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AutoMapper;
using Newtonsoft.Json;
using WebApp.Models;

namespace WebApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly IMapper _mapper;

        public HomeController(IMapper mapper)
        {
            _mapper = mapper;
        }

        public ActionResult Index()
        {
            var userList = UserData.UserList();
            var dtos = _mapper.Map<IEnumerable<UserDto>>(userList);
            var str1 = JsonConvert.SerializeObject(dtos);

            var user = userList.FirstOrDefault();
            var dto = _mapper.Map<UserDto>(user);
            var str2 = JsonConvert.SerializeObject(dto);
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