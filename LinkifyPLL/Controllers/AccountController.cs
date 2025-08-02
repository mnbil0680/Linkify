using LinkifyBLL.ModelView;
using LinkifyBLL.Services.Abstraction;
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

        public IActionResult Index(){
            return RedirectToAction("Login");
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
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var result = await IUS.RegisterUserAsync(model, model.Password);
            if (!result)
            {
                ModelState.AddModelError("", "Signup failed. Please try again.");
                return View(model);
            }

            return RedirectToAction("Login");
        }

       
    }

}
