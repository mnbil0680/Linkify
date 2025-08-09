using LinkifyDAL.DataBase;
using LinkifyDAL.Entities;
using LinkifyDAL.Enums;
using LinkifyDAL.Repo.Abstraction;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace LinkifyDAL.Repo.Implementation
{
    public class UserRepository : IUserRepository
    {
        private readonly UserManager<User> _userManager;
        private readonly LinkifyDbContext _db;

        public UserRepository(UserManager<User> userManager, LinkifyDbContext db)
        {
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
        }

        public async Task<IdentityResult> CreateUserAsync(User user, string password)
        {
            if (user == null) throw new ArgumentNullException(nameof(user));
            return await _userManager.CreateAsync(user, password);
        }

        public async Task<User> FindByEmailAsync(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                throw new ArgumentException("Email cannot be empty");

            return await _userManager.FindByEmailAsync(email);
        }

        public async Task<bool> CheckPasswordAsync(User user, string password)
        {
            if (user == null) throw new ArgumentNullException(nameof(user));
            return await _userManager.CheckPasswordAsync(user, password);
        }

        public async Task<IdentityResult> UpdateUserAsync(User user)
        {
            if (user == null) throw new ArgumentNullException(nameof(user));
            return await _userManager.UpdateAsync(user);
        }

        public async Task<IdentityResult> ChangePasswordAsync(User user, string currentPassword, string newPassword)
        {
            if (user == null) throw new ArgumentNullException(nameof(user));
            return await _userManager.ChangePasswordAsync(user, currentPassword, newPassword);
        }

        public async Task<bool> SoftDeleteUserAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null || user.IsDeleted) return false;

            user.Delete();
            var result = await _userManager.UpdateAsync(user);
            return result.Succeeded;
        }

        public async Task<bool> UpdateUserProfileAsync(string userId, string? userName, string? imgPath, string? cvPath, string? title, string? bio)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null || user.IsDeleted) return false;

            user.Edit(userName, imgPath, cvPath, title, bio);

            var result = await _userManager.UpdateAsync(user);
            return result.Succeeded;
        }

        public async Task<bool> UpdateUserStatusAsync(string userId, UserStatus newStatus)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return false;

            user.UpdateStatus(newStatus);
            var result = await _userManager.UpdateAsync(user);
            return result.Succeeded;
        }

        public async Task<User> GetUserByIdAsync(string userId)
        {
            if (string.IsNullOrWhiteSpace(userId))
                throw new ArgumentException("User ID cannot be empty");

            return await _userManager.FindByIdAsync(userId);
        }

        public async Task<bool> UserExistsAsync(string userId)
        {
            if (string.IsNullOrWhiteSpace(userId)) return false;
            return (await _userManager.FindByIdAsync(userId)) != null;
        }


    }
}
