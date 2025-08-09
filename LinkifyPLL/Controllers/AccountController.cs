using LinkifyBLL.ModelView;
using LinkifyBLL.Services.Abstraction;
using LinkifyDAL.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LinkifyPLL.Controllers
{
    public class AccountController : Controller
    {
        public readonly IUserService IUS;
        private readonly IAuthenticationService _authService;

        // Constructor to initialize IUserService
        public AccountController(IUserService ius, IAuthenticationService authService)
        {
            this.IUS = ius;
            this._authService = authService;
        }

        [Authorize]
        public IActionResult Index(){
            
            return View();
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(UserLoginMV model){
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
            
            ////////////////////////
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

            return RedirectToAction("SuccessfulRegister");
        }


        [HttpGet]
        public async Task<IActionResult> VerifyEmail()
        {
            return View();
        }


       public IActionResult SuccessfulRegister(UserRegisterMV model)
       {
            return View("SuccessfulRegister",model);

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
