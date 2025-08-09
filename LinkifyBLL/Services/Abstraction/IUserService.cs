using LinkifyBLL.ModelView;
using LinkifyDAL.Entities;
using LinkifyDAL.Enums;
using Microsoft.AspNetCore.Identity;

namespace LinkifyBLL.Services.Abstraction
{
    public interface IUserService
    {
        Task<IdentityResult> RegisterUserAsync(User user, string password);
        Task<User?> AuthenticateAsync(string email, string password);
        Task<IdentityResult> ChangePasswordAsync(string userId, string currentPassword, string newPassword);
        Task<bool> UpdateProfileAsync(string userId, string? userName, string? imgPath, string? cvPath, string? title, string? bio);
        Task<bool> UpdateStatusAsync(string userId, UserStatus status);
        Task<bool> DeleteUserAsync(string userId);
        Task<User?> GetUserByIdAsync(string userId);
        Task<User?> GetUserByEmailAsync(string email);
        Task<bool> UserExistsAsync(string userId);
    }
}
