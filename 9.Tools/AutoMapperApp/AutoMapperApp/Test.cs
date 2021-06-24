using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using AutoMapper;
using Bogus;
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
                var user = new Faker<UserEntity>()
                    .RuleFor(u => u.Id, f => Guid.NewGuid().ToString())
                    .RuleFor(u => u.Name, f => f.Name.FullName())
                    .RuleFor(u => u.Address, f => f.Address.CardinalDirection())
                    .RuleFor(u => u.Birthday, f => DateTime.Now)
                    .Generate();

                var userEntity = _mapper.Map<UserEntity>(user);
                Console.WriteLine(JsonConvert.SerializeObject(userEntity));
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        private List<UserEntity> UserList()
        {
            var users = new List<UserEntity>();
            for (var i = 1; i <= 5; i++)
            {
                var user = new Faker<UserEntity>()
                    .RuleFor(u => u.Id, f => Guid.NewGuid().ToString())
                    .RuleFor(u => u.Name, f => f.Name.FullName())
                    .RuleFor(u => u.Address, f => f.Address.CardinalDirection())
                    .RuleFor(u => u.Birthday, f => DateTime.Now)
                    .Generate();
                users.Add(user);
            }
            return users;
        }
    }

}
