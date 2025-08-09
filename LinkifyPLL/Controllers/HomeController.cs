using LinkifyBLL.ModelView;
using LinkifyBLL.Services.Abstraction;
using LinkifyDAL.Entities;
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
        public readonly IPostService IPS;
        public readonly IPostCommentsService IPCS;
        public readonly IPostImagesService IPIS;
        public readonly IPostReactionsService IPRS;
        public HomeController(ILogger<HomeController> logger, IFriendsService ifs, IPostService ips, IPostCommentsService ipcs, IPostImagesService ipis, IPostReactionsService iprs)
        {
            _logger = logger;
            this._IFS = ifs;
            this.IPS = ips;
            this.IPCS = ipcs;
            this.IPIS = ipis;
            this.IPRS = iprs;
        }

        public async Task<IActionResult> Index()
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

            // Fetch posts and related data asynchronously
            //var posts = (await IPS.GetRecentPostsAsync()).ToList();
            //var postImages = IPIS.GetImageByPostIdAsync(postID)
            //var comments = IPCS.GetCommentsForPostAsync(PostId);
            //var reactions = IPRS.
            //var shares = new List<SharePost>(); // Populate as needed

            var homeMV = MapHomeModelView(posts, postImages, comments, reactions, shares);
            return View(homeMV);
        }

        public HomeMV MapHomeModelView(
            List<Post> posts,
            List<PostImages> postImages,
            List<PostComments> comments,
            List<PostReactions> reactions,
            List<SharePost> shares)
        {
            return new HomeMV
            {
                Posts = posts,
                PostImages = postImages,
                PostComments = comments,
                PostReactions = reactions,
                SharePosts = shares
            };
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

        //public IActionResult MyConnections()
        //{
        //    var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        //    var friends = _IFS.GetFriends(userId).ToList();
        //    return View(friends);
        //}

        public IActionResult MyConnections()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var poepleList = _IFS.MyConnections(userId);
            return View(poepleList); 
        }
    }
}
