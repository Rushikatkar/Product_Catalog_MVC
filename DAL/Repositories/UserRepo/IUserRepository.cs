using DAL.DTOs;
using DAL.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Repositories.UserRepo
{
    public interface IUserRepository
    {
        Task<IEnumerable<UserDTO>> GetAllUsersAsync();
        Task<User> RegisterUserAsync(RegisterUserDTO registerUserDTO);

        Task<User> UpdateUserAsync(UpdateUserDTO updateUserDTO);

        Task<User> LoginUserAsync(LoginUserDTO loginUserDTO);

        Task<bool> DeleteUserAsync(int userId);

        Task<User> GetUserByIdAsync(int userId);
    }
}
