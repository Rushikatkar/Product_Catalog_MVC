using DAL.DTOs;
using DAL.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BAL.Services.UserService
{
    public interface IUserService
    {
        Task<IEnumerable<UserDTO>> GetAllUsersAsync();
        Task<User> RegisterUserAsync(RegisterUserDTO registerUserDTO);
        Task<User> UpdateUserAsync(UpdateUserDTO updateUserDTO);
        Task<string> LoginUserAsync(LoginUserDTO loginUserDTO); // JWT token as string
        Task<bool> DeleteUserAsync(int userId);
        Task<User> GetUserByIdAsync(int userId);
    }
}
