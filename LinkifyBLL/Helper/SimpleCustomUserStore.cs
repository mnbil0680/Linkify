using LinkifyDAL.DataBase;
using LinkifyDAL.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace LinkifyBLL.Helper
{
    public class CustomUserStore : UserStore<User, IdentityRole, LinkifyDbContext>
    {
        public CustomUserStore(LinkifyDbContext context, IdentityErrorDescriber describer = null)
            : base(context, describer)
        {
        }

        public override async Task<User> FindByNameAsync(string normalizedUserName, CancellationToken cancellationToken = default)
        {
            return null;
        }
        public async Task<User> FindByNameForLoginAsync(string normalizedUserName, CancellationToken cancellationToken = default)
        {
            return await base.FindByNameAsync(normalizedUserName, cancellationToken);
        }
    }
}