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
        private readonly ILogger<HomeController> _logger;
        public readonly IFriendsService _IFS;
        public HomeController(ILogger<HomeController> logger, IFriendsService ifs)
        {
            _logger = logger;
            this._IFS = ifs;
        }

        public IActionResult Index()
        {
            return View();
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
