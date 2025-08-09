using LinkifyDAL.Entities;
using LinkifyDAL.Enums;
using Microsoft.AspNetCore.Identity;
namespace LinkifyDAL.Repo.Abstraction
{
    public interface IUserRepository
    {
        Task<IdentityResult> CreateUserAsync(User user, string password);
        Task<User> FindByEmailAsync(string email);
        Task<bool> CheckPasswordAsync(User user, string password);
        Task<IdentityResult> UpdateUserAsync(User user);
        Task<IdentityResult> ChangePasswordAsync(User user, string currentPassword, string newPassword);
        Task<bool> SoftDeleteUserAsync(string userId);
        Task<bool> UpdateUserProfileAsync(string userId, string? userName, string? imgPath, string? cvPath, string? title, string? bio);
        Task<bool> UpdateUserStatusAsync(string userId, UserStatus newStatus);
        Task<User> GetUserByIdAsync(string userId);
        Task<bool> UserExistsAsync(string userId);
    }
}
