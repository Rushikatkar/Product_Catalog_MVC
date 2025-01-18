using DAL.DTOs;
using DAL.Models;
using DAL.Models.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Repositories.UserRepo
{
    public class UserRepository : IUserRepository
    {
        private readonly ApplicationDbContext _context;

        public UserRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<UserDTO>> GetAllUsersAsync()
        {
            return await _context.Users
                                 .Select(u => new UserDTO
                                 {
                                     UserId = u.UserId,
                                     Email = u.Email,
                                     Username = u.Username,
                                     Role = u.Role,
                                 })
                                 .ToListAsync();
        }

        // Register a new user
        public async Task<User> RegisterUserAsync(RegisterUserDTO registerUserDTO)
        {
            // Check if the user already exists with the same email
            var existingUser = await _context.Users
                .FirstOrDefaultAsync(u => u.Email == registerUserDTO.Email);

            if (existingUser != null)
            {
                throw new Exception("User with this email already exists.");
            }

            // Create a new User entity from the DTO
            var user = new User
            {
                Email = registerUserDTO.Email,
                Username = registerUserDTO.Username,
                PasswordHash = registerUserDTO.PasswordHash,  // Password should be hashed before storing
                Role = registerUserDTO.Role
            };

            // Add user to the database
            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();

            return user;
        }

        // Update an existing user
        public async Task<User> UpdateUserAsync(UpdateUserDTO updateUserDTO)
        {
            // Find the user by ID
            var user = await _context.Users.FindAsync(updateUserDTO.UserId);

            if (user == null)
            {
                throw new Exception("User not found.");
            }

            // Update user details
            if (!string.IsNullOrEmpty(updateUserDTO.Email))
                user.Email = updateUserDTO.Email;

            if (!string.IsNullOrEmpty(updateUserDTO.PasswordHash))
                user.PasswordHash = updateUserDTO.PasswordHash;  // You should hash the password before updating

            if (!string.IsNullOrEmpty(updateUserDTO.Role))
                user.Role = updateUserDTO.Role;

            // Save changes to the database
            _context.Users.Update(user);
            await _context.SaveChangesAsync();

            return user;
        }

        // Login a user
        public async Task<User> LoginUserAsync(LoginUserDTO loginUserDTO)
        {
            // Find the user by email
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Email == loginUserDTO.Email);

            if (user == null)
            {
                throw new Exception("Invalid credentials.");
            }



            return user;
        }

        // Delete a user by user ID
        public async Task<bool> DeleteUserAsync(int userId)
        {
            // Find the user by ID
            var user = await _context.Users.FindAsync(userId);

            if (user == null)
            {
                return false; // User not found
            }

            // Remove user from the database
            _context.Users.Remove(user);
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<User> GetUserByIdAsync(int userId)
        {
            // Find and return the user by user ID
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.UserId == userId);

            if (user == null)
            {
                throw new Exception("User not found.");
            }

            return user;
        }
    }
}
