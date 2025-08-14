using LinkifyBLL.ModelView;
using LinkifyBLL.Services.Abstraction;
using LinkifyDAL.Entities;
using LinkifyDAL.Enums;
using LinkifyPLL.Models;
using Microsoft.AspNetCore.Authorization;
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
        public readonly ISharePostService ISharePS;
        public HomeController(ILogger<HomeController> logger, IFriendsService ifs, IPostService ips, IPostCommentsService ipcs, IPostImagesService ipis, IPostReactionsService iprs, ISharePostService ishareps )
        {
            _logger = logger;
            this._IFS = ifs;
            this.IPS = ips;
            this.IPCS = ipcs;
            this.IPIS = ipis;
            this.IPRS = iprs;
            this.ISharePS = ishareps;
        }

        public async Task<IActionResult> Index()
        {

            List<PostMV> HomePosts = new List<PostMV>();
            var posts = (await IPS.GetRecentPostsAsync()).ToList();
            foreach (var post in posts)
            {
                var postMV = new PostMV
                {
                    PostUserName = post.User.UserName,
                    PostUserTitle = post.User.Title,
                    PostUserImg = post.User.ImgPath,
                    TextContent = post.TextContent,
                    Since = DateTime.Now - post.CreatedOn,
                    Images = new List<string>(),
                    imageCount = await IPIS.GetImageCountForPostAsync(post.Id),
                    CommentsCount = await IPCS.GetCommentCountForPostAsync(post.Id),
                    //Comments = new list<>
                    ReactionCount = await IPRS.GetReactionCountAsync(post.Id),
                    LikeCount = await IPRS.GetReactionCountAsync(post.Id, ReactionTypes.Like),
                    LoveCount = await IPRS.GetReactionCountAsync(post.Id, ReactionTypes.Love),
                    LaughCount = await IPRS.GetReactionCountAsync(post.Id, ReactionTypes.Haha),
                    SadCount = await IPRS.GetReactionCountAsync(post.Id, ReactionTypes.Sad),
                    AngryCount = await IPRS.GetReactionCountAsync(post.Id, ReactionTypes.Angry),
                    NumberOfShares = await ISharePS.GetPostShareCountAsync(post.Id)
                };

                var images = await IPIS.GetImageByPostIdAsync(post.Id);
                foreach (var img in images)
                {
                    postMV.Images.Add(img.ImagePath);
                }

                HomePosts.Add(postMV);
            }

            

            return View(HomePosts);
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
            var users = _IFS.GetAllUsersAsync().Result.ToList();
            return View(users);
        }

        public IActionResult PeopleYouMayKnow()
        {
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var peopleYouMayKnow = _IFS.GetPeopleYouMayKnowAsync(currentUserId).Result.ToList();
            return View(peopleYouMayKnow);
        }

        //public IActionResult MyConnections()
        //{
        //    var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        //    var friends = _IFS.GetFriends(userId).ToList();
        //    return View(friends);
        //}

        public async Task<IActionResult> MyConnections()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var poepleList = await _IFS.GetFriendsAsync(userId);
            return View(poepleList); 
        }

        public async Task <IActionResult> Invitation()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var pendingRequests = await _IFS.GetPendingRequestsAsync(userId);
            var model = pendingRequests
                .Where(pr => pr.Requester != null )
                .Select(pr => new ManageUser
            {
                UserId = pr.RequesterId,
                FullName = pr.Requester.UserName,
                AvatarUrl = pr.Requester.ImgPath,
                Status = FriendStatus.Pending,
                Since = pr.RequestDate
            }).ToList();
            return View(model);
        }
    }
}
