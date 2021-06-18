using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EasyStore.Application.User.Dto;
using Volo.Abp.Application.Services;

namespace EasyStore.Application.User
{
    public interface IUserService : IApplicationService
    {
        Task<bool> AddUser(UserDto userDto);

        Task<UserDto> GetUser(string userName);

        Task<List<UserDto>> GetUsers();

        Task<UserDto> UpdateUser(int id, string address);

        Task DeleteUser(int id);
    }
}
