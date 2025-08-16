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
        public static void LinkifyEnhancedConnectionString(this IServiceCollection services, IConfiguration configuration, string stringName = "defaultConnection")
        {
            var connectionString = configuration.GetConnectionString(stringName);
            services.AddDbContext<LinkifyDbContext>(options =>
                options.UseSqlServer(connectionString));
        }
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
                options.User.RequireUniqueEmail = true;
            })
            .AddEntityFrameworkStores<LinkifyDbContext>()
            .AddDefaultTokenProviders();

            services.Configure<IdentityOptions>(options =>
            {
                options.User.AllowedUserNameCharacters =
                    "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+ ";
            });
            services.AddScoped<IUserStore<User>, CustomUserStore>();
        }

        public static void LinkifyDependencyInjection(this IServiceCollection services) { 
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IAuthenticationService, AuthenticationService>();
            services.AddScoped<IFriendsRepository, FriendsRepository>();
            services.AddScoped<IFriendsService, FriendsService>();
            services.AddScoped<IPostService, PostService>();
            services.AddScoped<IPostRepository, PostRepository>();
            services.AddScoped<IPostImagesService, PostImagesService>();
            services.AddScoped<IPostImagesRepository, PostImagesRepository>();
            services.AddScoped<IPostReactionsService, PostReactionsService>();
            services.AddScoped<IPostReactionsRepository, PostReactionsRepository>();
            services.AddScoped<IPostCommentsService, PostCommentsService>();
            services.AddScoped<IPostCommentsRepository, PostCommentsRepository>();
            services.AddScoped<ISavePostService, SavePostService>();
            services.AddScoped<ISavePostRepository, SavePostRepository>();
            services.AddScoped<ISharePostService, SharePostService>();
            services.AddScoped<ISharePostRepository, SharePostRepository>();
            services.AddScoped<IContactService, ContactService>();
            services.AddScoped<IContactRepository, ContactRepository>();
            services.AddScoped<IJobService, JobService>();
            services.AddScoped<IJobRepository, JobRepository>();
            services.AddScoped<IJobApplicationService, JobApplicationService>();
            services.AddScoped<IJobApplicationRepository, JobApplicationRepository>();
            services.AddScoped<ISaveJobService, SaveJobService>();
            services.AddScoped<ISaveJobRepository, SaveJobRepository>();
            services.AddScoped<ICommentReactionsService, CommentReactionsService>();
            services.AddScoped<ICommentReactionsRepository, CommentReactionsRepository>();
            services.AddScoped<IEmailService, EmailService>();
            services.AddScoped<IUserSkillsService, UserSkillsService>();
            services.AddScoped<IUserSkillsRepository, UserSkillsRepository>();
            services.AddScoped<ICertificateRepository, CertificateRepository>();
            services.AddScoped<ICertificateService, CertificateService>();
        }
    }
}
