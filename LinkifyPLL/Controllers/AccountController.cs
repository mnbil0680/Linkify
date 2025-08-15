using LinkifyBLL.ModelView;
using LinkifyBLL.Services.Abstraction;
using LinkifyDAL.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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

        // Constructor to initialize IUserService
        public AccountController(IUserService ius, IAuthenticationService authService, IFriendsService ifs, IPostService ips, IEmailService ies)
        {
            this.IUS = ius;
            this._authService = authService;
            this.IFS = ifs;
            this.IPS = ips;
            this.IES = ies;
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
                LikesCount = 0,
                CommentsCount = 0, 
                SharesCount = 0,

                // Collections - initialize as empty (populated later from other services)
                Skills = new List<SkillMV>(),
                Languages = new List<LanguageMV>(),
                Certifications = new List<CertificationMV>(),
                WorkExperience = new List<ExperienceItemMV>(),
                VolunteerExperience = new List<VolunteerExperienceMV>(),
                Education = new List<EducationItemMV>(),
                Courses = new List<CourseMV>(),
                Projects = new List<ProjectItemMV>(),
                Posts = new List<PostMV>(),


                Connections = (await IFS.GetFriendsAsync(user.Id)).ToList()
            };
        }


        [HttpGet]
        public IActionResult Login()
        {
            return View();
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
