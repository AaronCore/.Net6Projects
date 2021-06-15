using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using AutoMapper;
using Newtonsoft.Json;

namespace AutoMapperApp
{
    public class Test
    {
        private readonly IMapper _mapper;
        public Test()
        {
            IConfigurationProvider config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<OrganizationProfile>();
            });
            _mapper = config.CreateMapper();
        }

        public void Test1()
        {
            Console.WriteLine("start test1:");
            try
            {
                var userList = UserList();
                var dtos = _mapper.Map<IEnumerable<UserDto>>(userList);
                Console.WriteLine(JsonConvert.SerializeObject(dtos));
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
        public void Test2()
        {
            Console.WriteLine("start test2:");
            try
            {
                var user = UserList().FirstOrDefault();
                var dto = _mapper.Map<UserDto>(user);
                Console.WriteLine(JsonConvert.SerializeObject(dto));
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
        public void Test3()
        {
            Console.WriteLine("start test3:");
            try
            {
                var dto = new UserDto
                {
                    UserId = Guid.NewGuid().ToString(),
                    UserName = "Jack",
                    Address = "福田",
                    Birthday = DateTime.Now
                };
                var userEntity = _mapper.Map<UserEntity>(dto);
                Console.WriteLine(JsonConvert.SerializeObject(userEntity));
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        private List<UserEntity> UserList()
        {
            var list = new List<UserEntity>();
            for (var i = 1; i <= 5; i++)
            {
                var entity = new UserEntity()
                {
                    Id = Guid.NewGuid().ToString(),
                    Name = $"测试{i}",
                    Address = $"深圳{i}",
                    Birthday = DateTime.Now
                };
                list.Add(entity);
            }
            return list;
        }
    }

}
