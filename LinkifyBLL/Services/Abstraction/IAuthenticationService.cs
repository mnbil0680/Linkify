using LinkifyDAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

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
