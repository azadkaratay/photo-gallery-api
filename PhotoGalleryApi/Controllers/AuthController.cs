using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PhotoGalleryApi.DTOs;
using PhotoGalleryApi.Entities;
using PhotoGalleryApi.Services.Interfaces;
using System.Security.Claims;

namespace PhotoGalleryApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IUserService _userService;

        public AuthController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<IActionResult> Register([FromBody] RegisterDto dto)
        {
            var user = await _userService.RegisterAsync(dto);
            if (user == null)
                return BadRequest(new { message = "Username already exists." });

            return Ok(new { message = "Registration successful." });
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            var token = await _userService.LoginAsync(dto);
            if (token == null)
                return Unauthorized(new { message = "Invalid username or password." });

            return Ok(new { token });
        }

        [HttpPut("change-password")]
        [Authorize]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDto dto)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");
            var success = await _userService.ChangePasswordAsync(userId, dto);

            if (!success)
                return BadRequest(new { message = "Password change failed. Please check your current password." });

            return Ok(new { message = "Password changed successfully." });
        }

        [HttpGet("me")]
        [Authorize]
        public async Task<IActionResult> GetMyInfo()
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");
            var user = await _userService.GetByIdAsync(userId);

            if (user == null)
                return NotFound(new { message = "User not found." });

            return Ok(new
            {
                user.Username,
                user.Role
            });
        }

        [HttpGet("all")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAll()
        {
            var users = await _userService.GetAllUsersAsync();

            return Ok(users);
        }

        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> Delete(int id)
        {
            var callerId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");
            var callerRole = User.FindFirstValue(ClaimTypes.Role) ?? "User";

            var success = await _userService.DeleteUserAsync(id, callerId, callerRole);
            if (!success)
                return StatusCode(403, new { message = "You are not authorized to delete this user." });

            return Ok(new { message = "User deleted successfully." });
        }
    }
}
