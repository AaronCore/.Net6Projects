using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SysEntity;
using SysEntityFrameworkCore;
using SysRepository;

namespace SysApplication.UserService
{
    public class UserService : IUserService
    {
        private readonly IRepository<UserEntity> _userRepository;
       
        public UserService(SysDbContext dbContext)
        {
            _userRepository = new Repository<UserEntity>(dbContext);
        }
        public void Add(UserEntity entity)
        {
            _userRepository.Insert(entity);
        }
    }
}
