using BCrypt.Net;
using DAL.DTOs;
using DAL.Models.Entities;
using DAL.Repositories.UserRepo;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace BAL.Services.UserService
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IConfiguration _configuration;

        public UserService(IUserRepository userRepository, IConfiguration configuration)
        {
            _userRepository = userRepository;
            _configuration = configuration;
        }

        public async Task<IEnumerable<UserDTO>> GetAllUsersAsync()
        {
            return await _userRepository.GetAllUsersAsync();
        }
        // Register a new user
        public async Task<User> RegisterUserAsync(RegisterUserDTO registerUserDTO)
        {
            try
            {
                // Create user entity from DTO
                var user = new RegisterUserDTO
                {
                    Email = registerUserDTO.Email,
                    Username = registerUserDTO.Username,
                    Role = registerUserDTO.Role
                };

                // Hash the password before storing it
                user.PasswordHash = GenerateHashedPassword(registerUserDTO.PasswordHash);

                // Register the user by passing the user entity to the repository
                return await _userRepository.RegisterUserAsync(user);
            }
            catch (Exception ex)
            {
                // Log or handle exception as needed
                throw new Exception("Error during registration: " + ex.Message);
            }
        }

        // Update an existing user
        public async Task<User> UpdateUserAsync(UpdateUserDTO updateUserDTO)
        {
            try
            {
                // Fetch the existing user
                var user = await _userRepository.GetUserByIdAsync(updateUserDTO.UserId);

                if (user == null)
                {
                    throw new Exception("User not found.");
                }

                var newuser = new UpdateUserDTO
                {
                    UserId = updateUserDTO.UserId != 0 ? updateUserDTO.UserId : user.UserId,
                    Email = updateUserDTO.Email ?? user.Email,
                    Role = updateUserDTO.Role ?? user.Role
                };
                //// Update user properties
                //user.Email = updateUserDTO.Email ?? user.Email;
                //user.Role = updateUserDTO.Role ?? user.Role;

                if (!string.IsNullOrEmpty(updateUserDTO.PasswordHash))
                {
                    // Hash the new password before updating
                    newuser.PasswordHash = GenerateHashedPassword(updateUserDTO.PasswordHash);
                }

                // Call the repository to update the user
                return await _userRepository.UpdateUserAsync(newuser);
            }
            catch (Exception ex)
            {
                // Log or handle exception as needed
                throw new Exception("Error during update: " + ex.Message);
            }
        }

        // Login a user and generate a JWT token
        public async Task<string> LoginUserAsync(LoginUserDTO loginUserDTO)
        {
            try
            {
                // Validate user credentials and login
                var user = await _userRepository.LoginUserAsync(loginUserDTO);

                if (user == null)
                {
                    throw new Exception("Invalid credentials.");
                }

                // Verify the password
                if (!VerifyPassword(user.PasswordHash, loginUserDTO.PasswordHash))
                {
                    throw new Exception("Invalid credentials.");
                }

                // Generate JWT token upon successful login
                return GenerateJwtToken(user);
            }
            catch (Exception ex)
            {
                // Log or handle exception as needed
                throw new Exception("Error during login: " + ex.Message);
            }
        }

        // Delete a user by user ID
        public async Task<bool> DeleteUserAsync(int userId)
        {
            try
            {
                // Delete the user by user ID
                return await _userRepository.DeleteUserAsync(userId);
            }
            catch (Exception ex)
            {
                // Log or handle exception as needed
                throw new Exception("Error during deletion: " + ex.Message);
            }
        }

        // Get a user by ID
        public async Task<User> GetUserByIdAsync(int userId)
        {
            try
            {
                // Fetch user by ID using the repository
                var user = await _userRepository.GetUserByIdAsync(userId);

                if (user == null)
                {
                    throw new Exception("User not found.");
                }

                return user;
            }
            catch (Exception ex)
            {
                // Log or handle exception as needed
                throw new Exception("Error during fetching user: " + ex.Message);
            }
        }

        // Helper method to generate the hashed password using BCrypt
        private string GenerateHashedPassword(string password)
        {
            // Hash the password using BCrypt
            return BCrypt.Net.BCrypt.HashPassword(password);
        }

        // Helper method to verify the hashed password using BCrypt
        private bool VerifyPassword(string storedHash, string inputPassword)
        {
            // Verify the password using BCrypt
            return BCrypt.Net.BCrypt.Verify(inputPassword, storedHash);
        }

        // Helper method to generate JWT token
        private string GenerateJwtToken(User user)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
                new Claim(ClaimTypes.Name, user.Email),
                new Claim(ClaimTypes.Role, user.Role),
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddDays(1),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
