using System;
using System.Collections.Generic;
using Bogus;
using WebApp.Models;

namespace WebApp
{
    public class UserData
    {
        public static List<UserEntity> UserList()
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