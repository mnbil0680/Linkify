using LinkifyBLL.ModelView;
using LinkifyBLL.Services.Abstraction;
using LinkifyDAL.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using System.Security.Claims;

namespace LinkifyPLL.Controllers
{
    public class AccountController : Controller
    {
        public readonly IUserService IUS;
        private readonly IAuthenticationService _authService;
        private readonly IFriendsService IFS;
        private readonly IPostService IPS;
        private readonly IEmailService IES;
        private readonly IPostCommentsService IPCS;
        private readonly IPostReactionsService IPostReactionS;
        private readonly ISharePostService ISharePostService;

        // Constructor to initialize IUserService
        public AccountController(IUserService ius, IAuthenticationService authService, IFriendsService ifs, IPostService ips, IEmailService ies, IPostReactionsService IPRS, ISharePostService IsharePost, IPostCommentsService ipcs )
        {
            this.IUS = ius;
            this._authService = authService;
            this.IFS = ifs;
            this.IPS = ips;
            this.IES = ies;
            this.IPostReactionS = IPRS;
            this.ISharePostService = IsharePost;
            this.IPCS = ipcs;
        }

        [Authorize]
        public async Task<IActionResult> Index()
        {
            // Get current user's ID from claims
            var userId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);

            // May be 404 Page
            if (string.IsNullOrEmpty(userId))
            {
               
                return RedirectToAction("Login");
            }

            // Get user from service (you'll need to add this method to IUserService)
            var user = await IUS.GetUserByIdAsync(userId);

            if (user == null)
            {
                return RedirectToAction("Login");
            }

            // Map User entity to ProfileMV
            var profileMV = await MapUserToProfileMVAsync(user);


            return View(profileMV);
        }

        public async Task<IActionResult> User(string UserId)
        {
            // May be 404 Page
            if (string.IsNullOrEmpty(UserId))
            {
                return RedirectToAction("Login");
            }

            var user = await IUS.GetUserByIdAsync(UserId);

            var profileMV =  await MapUserToProfileMVAsync(user); // Added await
            return View("index", profileMV);
        }

        // Manual Mapping
        private async Task<ProfileMV> MapUserToProfileMVAsync(User user)
        {
            List<Post> UserPosts = (List<Post>)await IPS.GetUserPostsAsync(user.Id);
            int TotalReactionNumber = 0;
            int TotalCommentsRecieved = 0;
            int TotalNumberShares = 0;
            foreach(var post in UserPosts)
            {
                TotalReactionNumber += await IPostReactionS.GetReactionCountAsync(post.Id);
                TotalCommentsRecieved += await IPCS.GetCommentCountForPostAsync(post.Id);
            }

            
            return new ProfileMV
            {
                // Basic Information (from User entity)
                Id = user.Id,
                Name = user.UserName ?? "Unknown User",
                Email = user.Email,
                PhoneNumber = user.PhoneNumber, // from IdentityUser
                ImgPath = user.ImgPath,
                Location = "Unknown Country - Unknown City",

                // Professional Information (from User entity)
                Title = user.Title,
                Bio = user.Bio,
                Status = user.Status.ToString(),

                // Properties not in User entity - set to null/defaults
                Company = "UnKnow Company",
                Industry = null,

                // Social Links - all null (not in User entity)
                LinkedInUrl = null,
                GitHubUrl = null,
                PortfolioUrl = null,

                // Professional Details - defaults
                YearsOfExperience = null,

                // Account & Activity Information
                RegistrationDate = user.RegistrationDate,
                IsPremiumMember = false, // default

                // Stats - set to 0 (you can calculate these later)
                ConnectionsCount = await IFS.GetFriendCountAsync(user.Id),
                PostsCount = await IPS.GetUserPostCountAsync(user.Id),
                ProfileViews = 0,
                LikesCount = TotalReactionNumber,
                CommentsCount = TotalCommentsRecieved,
                SharesCount = await ISharePostService.GetUserShareCountAsync(user.Id),

                // Collections - initialize as empty (populated later from other services)
                Skills = new List<SkillMV>(),
                Languages = new List<LanguageMV>(),
                Certifications = new List<CertificationMV>(),
                WorkExperience = new List<ExperienceItemMV>(),
                VolunteerExperience = new List<VolunteerExperienceMV>(),
                Education = new List<EducationItemMV>(),
                Courses = new List<CourseMV>(),
                Projects = new List<ProjectItemMV>(),
                Posts = (await IPS.GetUserPostsAsync(user.Id)).Take(3).ToList(),



                Connections = (await IFS.GetFriendsAsync(user.Id)).ToList()
            };
        }


        [HttpGet]
        public async Task<IActionResult> Login(string returnUurl)
        {
            UserLoginMV model = new UserLoginMV
            {
                ReturnUrl = returnUurl,
                // Corrected the usage of SignInManager to include the generic type argument
                ExternalLogins = (await HttpContext.RequestServices
                    .GetRequiredService<SignInManager<User>>()
                    .GetExternalAuthenticationSchemesAsync()).ToList()
            };
            return View(model);
        }

        [AllowAnonymous]
        [HttpPost]
        public IActionResult ExternalLogin(string provider, string returnUrl)
        {
            var redirectUrl = Url.Action("ExternalLoginCallback", "Account", new { ReturnUrl = returnUrl });

            var properties = HttpContext.RequestServices
                    .GetRequiredService<SignInManager<User>>().ConfigureExternalAuthenticationProperties(provider, redirectUrl);
            return new ChallengeResult(provider, properties);


        }

        // Fix for CS7036, CS1922, and CS0103 errors in the ExternalLoginCallback method
        [AllowAnonymous]
        public async Task<IActionResult> ExternalLoginCallback(string returnUrl = null, string remoteError = null)
        {
            returnUrl = returnUrl ?? Url.Content("~/");

            UserLoginMV loginMV = new UserLoginMV
            {
                ReturnUrl = returnUrl,
                ExternalLogins = (await HttpContext.RequestServices
                    .GetRequiredService<SignInManager<User>>()
                    .GetExternalAuthenticationSchemesAsync()).ToList(),
            };

            if (remoteError != null)
            {
                ModelState.AddModelError(string.Empty, $"Error From External Provider: {remoteError}");
                return View("Login", loginMV);
            }

            var info = await HttpContext.RequestServices
                .GetRequiredService<SignInManager<User>>().GetExternalLoginInfoAsync();

            if (info == null)
            {
                ModelState.AddModelError(string.Empty, "Error Loading External Login Information");
                return View("Login", loginMV);
            }

            var signInResult = await HttpContext.RequestServices
                .GetRequiredService<SignInManager<User>>()
                .ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, isPersistent: false, bypassTwoFactor: true);

            if (signInResult.Succeeded)
            {
                return LocalRedirect(returnUrl);
            }
            else
            {
                var email = info.Principal.FindFirstValue(ClaimTypes.Email);
                if (email != null)
                {
                    var user = await IUS.GetUserByEmailAsync(email);
                    if (user == null)
                    {
                        // Fix for CS7036: Provide required parameters for the User constructor
                        user = new User(email, email, null, null, null, null, null);

                        await HttpContext.RequestServices.GetRequiredService<UserManager<User>>().CreateAsync(user);
                    }

                    // Fix for CS0103: AccessFailedCount is a property of the User class, not a standalone variable
                    user.AccessFailedCount = 0;

                    // Fix for CS1922: Use the UserManager to add the login information
                    await HttpContext.RequestServices.GetRequiredService<UserManager<User>>().AddLoginAsync(user, info);
                    await HttpContext.RequestServices.GetRequiredService<SignInManager<User>>().SignInAsync(user, isPersistent: false);
                    return LocalRedirect(returnUrl);
                }

                ViewBag.ErrorTitle = $"Email claim not received  from: {info.LoginProvider}";
                ViewBag.ErrorMessage = "Please contact support on linkifycorprate@gmail.com";
                return View("Error");
            }
            return View("Login", loginMV);
        }


        [HttpPost]
        public async Task<IActionResult> Login(UserLoginMV model)
        {
            var user = await _authService.LoginAsync(model.Email, model.Password, true);
            if (user == null)
            {
                ModelState.AddModelError("", "Invalid credentials");
                return View(model);
            }
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public async Task<IActionResult> Signup()
        {

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Signup(UserRegisterMV model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            User us = new User(model.Name, model.Email, null, null, null, null, null);
            var result = await IUS.RegisterUserAsync(us, model.Password);
            if (!result.Succeeded)
            {
                ModelState.AddModelError("", "Signup failed. Please try again.");
                return View(model);
            }

            string EmailTitle = "Welcome To Linkify";
            await IES.SendEmail(model.Email, EmailTitle); //SendEmail

            return RedirectToAction("SuccessfulRegister", model);
        }


        [HttpGet]
        public async Task<IActionResult> VerifyEmail()
        {
            return View();
        }


        public IActionResult SuccessfulRegister(UserRegisterMV model)
        {

            return View("SuccessfulRegister", model);

        }

        [HttpGet]
        public async Task<IActionResult> ChangePassword()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await _authService.LogoutAsync();
            return RedirectToAction("Login");
        }
    }
}
