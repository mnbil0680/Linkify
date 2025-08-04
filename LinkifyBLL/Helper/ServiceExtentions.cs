using LinkifyBLL.Services.Abstraction;
using LinkifyBLL.Services.Implementation;
using LinkifyDAL.DataBase;
using LinkifyDAL.Entities;
using LinkifyDAL.Repo.Abstraction;
using LinkifyDAL.Repo.Implementation;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace LinkifyBLL.Helper
{
    public static class ServiceExtensions
    {
        public static void LinkifyIdentity(this IServiceCollection services)
        {
            services.AddIdentity<User, IdentityRole>(options =>
            {
                options.SignIn.RequireConfirmedAccount = false;
                options.Password.RequireDigit = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = true;
                options.Password.RequiredLength = 8;
                
            })
            .AddEntityFrameworkStores<LinkifyDbContext>()
            .AddDefaultTokenProviders();
            services.Configure<IdentityOptions>(options =>
            {
                options.User.AllowedUserNameCharacters =
                    "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+ ";
            });
        }

        public static void LinkifyDependencyInjection(this IServiceCollection services) { 
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IAuthenticationService, AuthenticationService>();
            services.AddScoped<IFriendsRepository, FriendsRepository>();
            services.AddScoped<IFriendsService, FriendsService>();
        }

        public static void LinkifyEnhancedConnectionString(this IServiceCollection services, IConfiguration configuration, string stringName = "defaultConnection")
        {
            var connectionString = configuration.GetConnectionString(stringName);
            services.AddDbContext<LinkifyDbContext>(options =>
                options.UseSqlServer(connectionString));
        }
    }
}
