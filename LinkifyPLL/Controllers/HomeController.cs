using LinkifyBLL.ModelView;
using LinkifyBLL.Services.Abstraction;
using LinkifyPLL.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Security.Claims;

namespace LinkifyPLL.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        public readonly IUserService IUS;
        private readonly ILogger<HomeController> _logger;
        public readonly IFriendsService _IFS;
        public HomeController(ILogger<HomeController> logger, IFriendsService ifs)
        {
            _logger = logger;
            this._IFS = ifs;
        }

        public IActionResult Index()
        {
            // Create UserMV from Identity claims
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var userName = User.Identity?.Name;
            var userEmail = User.FindFirstValue(ClaimTypes.Email);

            var userMV = new UserMV(
                id: userId ?? string.Empty,
                name: userName ?? string.Empty,
                email: userEmail ?? string.Empty,
                password: string.Empty, // Don't pass password
                imgPath: null, // You might want to get this from database
                status: null   // You might want to get this from database
            );


            return View(userMV);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public IActionResult People()
        {
            var users = _IFS.GetAllUsers().ToList();
            return View(users);
        }

        public IActionResult PeopleYouMayKnow()
        {
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var peopleYouMayKnow = _IFS.GetPeopleYouMayKnow(currentUserId).ToList();
            return View(peopleYouMayKnow);
        }
    }
}
