using PhotoGalleryApi.DTOs;
using PhotoGalleryApi.Entities;

namespace PhotoGalleryApi.Services.Interfaces
{
    public interface IUserService
    {
        Task<User?> RegisterAsync(RegisterDto dto);
        Task<string?> LoginAsync(LoginDto dto);
        Task<bool> ChangePasswordAsync(int userId, ChangePasswordDto dto);
        Task<User?> GetByIdAsync(int userId);
        Task<List<UserResponseDto>> GetAllUsersAsync();
        Task<bool> DeleteUserAsync(int userId, int callerId, string callerRole);
    }
}
