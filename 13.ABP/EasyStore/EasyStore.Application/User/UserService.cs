using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EasyStore.Application.User.Dto;
using EasyStore.Domain;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;

namespace EasyStore.Application.User
{
    public class UserService : ApplicationService, IUserService
    {
        private readonly IRepository<Users> _userRepository;
        public UserService(IRepository<Users> userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<bool> AddUser(UserDto userDto)
        {
            var entity = ObjectMapper.Map<UserDto, Users>(userDto);
            var insert = await _userRepository.InsertAsync(entity, true);
            return insert != null;
        }

        public async Task<UserDto> GetUser(string userName)
        {
            var user = await _userRepository.FirstOrDefaultAsync(p => p.Name.Contains(userName));
            var userDto = ObjectMapper.Map<Users, UserDto>(user);
            return userDto;
        }

        public async Task<List<UserDto>> GetUsers()
        {
            var users = await _userRepository.GetListAsync();
            var userDtos = ObjectMapper.Map<List<Users>, List<UserDto>>(users);
            return userDtos;
        }

        public async Task<UserDto> UpdateUser(int id, string address)
        {
            var userModel = await _userRepository.FindAsync(p => p.Id == id);
            userModel.Address = address;
            var updateUser = await _userRepository.UpdateAsync(userModel, true);

            var userDto = ObjectMapper.Map<Users, UserDto>(updateUser);
            return userDto;
        }

        public async Task DeleteUser(int id)
        {
            await _userRepository.DeleteAsync(p => p.Id == id, true);
        }
    }
}
