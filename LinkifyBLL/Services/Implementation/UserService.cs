using LinkifyBLL.ModelView;
using LinkifyBLL.Services.Abstraction;
using LinkifyDAL.Entities;
using LinkifyDAL.Repo.Abstraction;

namespace LinkifyBLL.Services.Implementation
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepo;

        public UserService(IUserRepository userRepo)
        {
            _userRepo = userRepo;
        }

        public async Task<bool> RegisterUserAsync(UserRegisterMV userMV, string password)
        {
            User user = new User(userMV.Name, userMV.Email, null, null);
            return await _userRepo.RegisterUserAsync(user, password);
        }

        public async Task<bool> DeleteUserAsync(UserMV userMV)
        {
            var user = await _userRepo.FindByEmailAsync(userMV.Email);
            if (user == null) return false;

            return await _userRepo.DeleteUserAsync(user);
        }

        public async Task<bool> UpdateUserAsync(UserMV oldUserMV, UserMV newUserMV)
        {
            var existingUser = await _userRepo.FindByEmailAsync(oldUserMV.Email);
            if (existingUser == null) return false;

            existingUser.Edit(newUserMV.Name, newUserMV.ImgPath, newUserMV.Status);
            var result = await _userRepo.UpdateUserAsync(existingUser);
            return result.Succeeded;
        }

        public async Task<bool> ChangeUserPasswordAsync(UserMV userMV, string oldPassword, string newPassword)
        {
            var user = await _userRepo.FindByEmailAsync(userMV.Email);
            if (user == null) return false;

            return await _userRepo.ChangeUserPasswordAsync(user, oldPassword, newPassword);
        }
    }
}

