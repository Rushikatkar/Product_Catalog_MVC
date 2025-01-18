using BAL.Services.UserService;
using DAL.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Presentation_Layer.Controllers
{
    [Route("user")]
    //[Authorize]
    public class UserController : Controller
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        // Action to fetch all users and display them in a view
        [HttpGet("users")]
        [AllowAnonymous]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = await _userService.GetAllUsersAsync();
            return View(users);
        }
        // Register a new user (using a View for registration)
        [HttpGet("register")]
        [AllowAnonymous]
        public IActionResult RegisterUser()
        {
            return View();
        }

        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<IActionResult> RegisterUser(RegisterUserDTO registerUserDTO)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var user = await _userService.RegisterUserAsync(registerUserDTO);
                    TempData["Message"] = "User registered successfully.";
                    return RedirectToAction("LoginUser");
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", ex.Message);
                }
            }

            return View(registerUserDTO);
        }

        // Login a user (using a View for login)
        [HttpGet("login")]
        [AllowAnonymous]
        public IActionResult LoginUser()
        {
            return View();
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> LoginUser(LoginUserDTO loginUserDTO)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var token = await _userService.LoginUserAsync(loginUserDTO);
                    // Store JWT token in session or cookies if needed
                    Response.Cookies.Append("AuthToken", token, new CookieOptions
                    {
                        HttpOnly = true,
                        Secure = true,
                        SameSite = SameSiteMode.Strict,
                        Expires = DateTime.UtcNow.AddHours(2)
                    });
                    TempData["Message"] = "Login successful.";
                    return RedirectToAction("allproducts", "products"); // Redirect to Home or Dashboard after login
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", ex.Message);
                }
            }

            return View(loginUserDTO);
        }

        // Update an existing user (using a View for updating)
        [HttpGet("update/{userId}")]
        public async Task<IActionResult> UpdateUser(int userId)
        {
            var user = await _userService.GetUserByIdAsync(userId);
            if (user == null)
            {
                return NotFound();
            }

            var updateUserDTO = new UpdateUserDTO
            {
                UserId = user.UserId,
                Email = user.Email,
                Username= user.Username,
                Role = user.Role
            };

            return View(updateUserDTO);
        }

        [HttpPost("update/{userId}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateUser(int userId, UpdateUserDTO updateUserDTO)
        {
            if (userId != updateUserDTO.UserId)
            {
                ModelState.AddModelError("", "UserId in the URL does not match the UserId in the form data.");
                return View(updateUserDTO);
            }

            if (ModelState.IsValid)
            {
                try
                {
                    await _userService.UpdateUserAsync(updateUserDTO);
                    TempData["Message"] = "User updated successfully.";
                    return RedirectToAction("Users", new { userId = updateUserDTO.UserId });
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", ex.Message);
                }
            }

            return View(updateUserDTO);
        }


        // Delete a user (redirect after deletion)
        [HttpGet("delete/{userId}")]
        public async Task<IActionResult> DeleteUser(int userId)
        {
            var isDeleted = await _userService.DeleteUserAsync(userId);
            if (!isDeleted)
            {
                TempData["Message"] = "User not found.";
                return RedirectToAction("Index", "Home");
            }

            TempData["Message"] = "User deleted successfully.";
            return RedirectToAction("Index", "Home");
        }

        // Get a user by ID (view user's profile or information)
        [HttpGet("get/{userId}")]
        public async Task<IActionResult> GetUserById(int userId)
        {
            var user = await _userService.GetUserByIdAsync(userId);
            if (user == null)
            {
                return NotFound();
            }

            return View(user); // Show the user information in a view
        }
    }
}
