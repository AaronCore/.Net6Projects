using System;
using System.Collections.Generic;
using WebApp.Models;

namespace WebApp
{
    public class UserData
    {
        public static List<UserEntity> UserList()
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