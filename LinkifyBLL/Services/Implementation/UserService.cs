using LinkifyBLL.ModelView;
using LinkifyBLL.Services.Abstraction;
using LinkifyDAL.Entities;
using LinkifyDAL.Enums;
using LinkifyDAL.Repo.Abstraction;
using Microsoft.AspNetCore.Identity;

namespace LinkifyBLL.Services.Implementation
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly UserManager<User> _userManager;

        public UserService(IUserRepository userRepository, UserManager<User> userManager)
        {
            _userRepository = userRepository;
            _userManager = userManager;
        }

        public async Task<IdentityResult> RegisterUserAsync(User user, string password)
        {
            if (user == null) throw new ArgumentNullException(nameof(user));
            return await _userRepository.CreateUserAsync(user, password);
        }

        public async Task<User?> AuthenticateAsync(string email, string password)
        {
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
                return null;

            var user = await _userRepository.FindByEmailAsync(email);
            if (user == null || user.IsDeleted) return null;

            return await _userRepository.CheckPasswordAsync(user, password) ? user : null;
        }

        public async Task<IdentityResult> ChangePasswordAsync(string userId, string currentPassword, string newPassword)
        {
            if (string.IsNullOrWhiteSpace(userId))
                throw new ArgumentException("User ID is required");

            var user = await _userRepository.GetUserByIdAsync(userId);
            if (user == null || user.IsDeleted)
                throw new InvalidOperationException("User not found");

            return await _userRepository.ChangePasswordAsync(user, currentPassword, newPassword);
        }

        public async Task<bool> UpdateProfileAsync(string userId, string? userName, string? imgPath, string? cvPath, string? title, string? bio)
        {
            if (string.IsNullOrWhiteSpace(userId))
                throw new ArgumentException("User ID is required");

            return await _userRepository.UpdateUserProfileAsync(userId, userName, imgPath, cvPath, title, bio);
        }

        public async Task<bool> UpdateStatusAsync(string userId, UserStatus status)
        {
            if (string.IsNullOrWhiteSpace(userId))
                throw new ArgumentException("User ID is required");

            return await _userRepository.UpdateUserStatusAsync(userId, status);
        }

        public async Task<bool> DeleteUserAsync(string userId)
        {
            if (string.IsNullOrWhiteSpace(userId))
                throw new ArgumentException("User ID is required");

            return await _userRepository.SoftDeleteUserAsync(userId);
        }

        public async Task<User?> GetUserByIdAsync(string userId)
        {
            if (string.IsNullOrWhiteSpace(userId)) return null;
            return await _userRepository.GetUserByIdAsync(userId);
        }

        public async Task<User?> GetUserByEmailAsync(string email)
        {
            if (string.IsNullOrWhiteSpace(email)) return null;
            return await _userRepository.FindByEmailAsync(email);
        }

        public async Task<bool> UserExistsAsync(string userId)
        {
            if (string.IsNullOrWhiteSpace(userId)) return false;
            return await _userRepository.UserExistsAsync(userId);
        }
    }
}

