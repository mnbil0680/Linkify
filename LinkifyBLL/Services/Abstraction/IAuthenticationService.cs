using LinkifyDAL.Entities;
using System.Security.Claims;

namespace LinkifyBLL.Services.Abstraction
{
    public interface IAuthenticationService
    {
        bool IsSignedIn(ClaimsPrincipal user);
        Task<User> LoginAsync(string email, string password, bool rememberMe);
        Task LogoutAsync();
        Task<bool> IsEmailConfirmedAsync(string email);
    }
}
