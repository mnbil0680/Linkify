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
        // POSTs
        public readonly IPostService IPS;
        public readonly IPostCommentsService IPCS;
        public readonly IPostImagesService IPIS;
        //Reactions
        public readonly IPostReactionsService IPRS;
        // Share
        public readonly ISharePostService ISharePS;
        // Comment
        public readonly ICommentReactionsService ICRS;

        // Save
        public readonly ISavePostService ISavePostS;
        private readonly IJobService _jobService;


        public HomeController(ILogger<HomeController> logger,IUserService ius, IFriendsService ifs, IPostService ips, IPostCommentsService ipcs, IPostImagesService ipis, IPostReactionsService iprs, ISharePostService ishareps, ICommentReactionsService icrs, ISavePostService isps , IJobService jobService)
        {
            _logger = logger;
            _jobService = jobService;
            this.IUS = ius;
            this._IFS = ifs;
            this.IPS = ips;
            this.IPCS = ipcs;
            this.IPIS = ipis;
            this.IPRS = iprs;
            this.ICRS = icrs;
            this.ISharePS = ishareps;
            this.ISavePostS = isps;
        }

        public async Task<IActionResult> Index()
        {

            
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

            // Get current user's avatar for ViewBag
            if (!string.IsNullOrEmpty(userId))
            {
                var currentUser = await IUS.GetUserByIdAsync(userId);
                ViewBag.CurrentUserAvatar = currentUser?.ImgPath ?? "/imgs/Account/default.png";
            }
            else
            {
                ViewBag.CurrentUserAvatar = "/imgs/Account/default.png";
            }
            // --- Prepare RightSide Data ---
            var suggestions = await _IFS.GetPeopleYouMayKnowAsync(userId);
            var connectionList = suggestions
                .Take(3)
                .Select(u => new RightSideConnection
                {
                    Id = u.Id,
                    ImgPath = u.ImgPath ?? "/imgs/Account/default.png",
                    Title = u.Title,
                    Name = u.UserName
                })
                .ToList();

            var jobs = await _jobService.GetAllJobsAsync();
            var jobList = jobs
                .Take(3)
                .Select(job => new RightSideJob
                {
                    Id = job.Id,
                    Title = job.Title,
                    Company = job.Company,
                    Location = job.Location,
                    Salary = job.SalaryRange,
                    Presence = job.Presence ?? JobPresence.Onsite
                })
                .ToList();

            var rightSideModel = new RightSide
            {
                Connections = connectionList,
                Jobs = jobList
            };

            // Store in ViewData
            ViewData["RightSideData"] = rightSideModel;
            List<PostMV> HomePosts = new List<PostMV>();

            // Map SharedPostMV
            List<SharePost> sharedPostsBeforeMap = (List<SharePost>)await ISharePS.GetUserSharesAsync(userId);
            sharedPostsBeforeMap.Reverse();
            foreach (var sharedpostData in sharedPostsBeforeMap)
            {
                // Get the original post data
                var originalPost = sharedpostData.Post;
                var sharedByUser = sharedpostData.User;

                PostMV sharedPostMV = new PostMV()
                {
                    // Original post data
                    
                    postId = sharedpostData.Id,
                    PostUserName = originalPost.User.UserName,
                    PostUserId = originalPost.UserId,
                    PostUserTitle = originalPost.User.Title,
                    PostUserImg = originalPost.User.ImgPath ?? "/imgs/Account/default.png",
                    TextContent = originalPost.TextContent,
                    CreatedAt = originalPost.CreatedOn,
                    IsEdited = (originalPost.UpdatedOn != null),
                    Since = DateTime.Now - originalPost.CreatedOn,

                    // Shared post specific data
                    IsSharedPost = true,
                    SharedById = sharedByUser, // User who shared the post
                    SharedPostAuthorId = originalPost.User, // Original post author
                    SharedCaption = sharedpostData.Caption,
                    SharedAt = sharedpostData.SharedAt,

                    // Initialize collections
                    Images = new List<string>(),
                    Comments = new List<CommentCreateMV>(),
                    Reactions = new List<PostReactionMV>(),

                    // Get counts and other data (same as regular posts)
                    imageCount = await IPIS.GetImageCountForPostAsync(originalPost.Id),
                    CommentsCount = await IPCS.GetCommentCountForPostAsync(originalPost.Id),
                    NumberOfShares = await ISharePS.GetPostShareCountAsync(originalPost.Id),
                    IsSavedByCurrentUser = await ISavePostS.IsPostSavedByUserAsync(originalPost.Id, userId),

                    // Reaction counts
                    ReactionCount = await IPRS.GetReactionCountAsync(originalPost.Id),
                    LikeCount = await IPRS.GetReactionCountAsync(originalPost.Id, ReactionTypes.Like),
                    LoveCount = await IPRS.GetReactionCountAsync(originalPost.Id, ReactionTypes.Love),
                    LaughCount = await IPRS.GetReactionCountAsync(originalPost.Id, ReactionTypes.Haha),
                    SadCount = await IPRS.GetReactionCountAsync(originalPost.Id, ReactionTypes.Sad),
                    AngryCount = await IPRS.GetReactionCountAsync(originalPost.Id, ReactionTypes.Angry),
                    ReactionsNumbers = new List<int>
        {
            await IPRS.GetReactionCountAsync(originalPost.Id, ReactionTypes.Like),
            await IPRS.GetReactionCountAsync(originalPost.Id, ReactionTypes.Love),
            await IPRS.GetReactionCountAsync(originalPost.Id, ReactionTypes.Haha),
            await IPRS.GetReactionCountAsync(originalPost.Id, ReactionTypes.Sad),
            await IPRS.GetReactionCountAsync(originalPost.Id, ReactionTypes.Angry)
        }
                };

                // Load images (same logic as regular posts)
                var images = await IPIS.GetImageByPostIdAsync(originalPost.Id);
                foreach (var img in images)
                {
                    sharedPostMV.Images.Add(img.ImagePath);
                }

                var comments = await IPCS.GetCommentsForPostAsync(originalPost.Id);
                foreach (var comment in comments)
                {
                    // Fetch reactions for this comment
                    var commentReactions = await ICRS.GetReactionsByCommentAsync(comment.Id);

                    var reactionMVs = commentReactions.Select(r => new CommentReactionMV
                    {
                        Id = r.Id,
                        CommentId = r.CommentId,
                        ReactorId = r.ReactorId,
                        ReactorUserName = r.Reactor?.UserName,
                        Reaction = r.Reaction.ToString(),
                        IsDeleted = r.IsDeleted,
                        CreatedOn = r.CreatedOn
                    }).ToList();

                    // Fetch replies for this comment
                    var replies = await IPCS.GetCommentRepliesAsync(comment.Id);

                    var replyMVs = new List<CommentCreateMV>();
                    foreach (var reply in replies)
                    {
                        // Fetch reactions for each reply
                        var replyReactions = await ICRS.GetReactionsByCommentAsync(reply.Id);
                        var replyReactionMVs = replyReactions.Select(r => new CommentReactionMV
                        {
                            Id = r.Id,
                            CommentId = r.CommentId,
                            ReactorId = r.ReactorId,
                            ReactorUserName = r.Reactor?.UserName,
                            Reaction = r.Reaction.ToString(),
                            IsDeleted = r.IsDeleted,
                            CreatedOn = r.CreatedOn
                        }).ToList();

                        // Create reply CommentCreateMV
                        replyMVs.Add(new CommentCreateMV(
                            isEdited: (reply.UpdatedOn != null ? true : false),
                            createdAt: reply.CreatedOn,
                            authorName: reply.User.UserName,
                            authorAvatar: reply.User.ImgPath,
                            commentId: reply.Id,
                            postId: reply.PostId,
                            textContent: reply.Content,
                            imagePath: reply.ImgPath,
                            parentCommentId: reply.ParentCommentId,
                            commenterId: reply.CommenterId
                        )
                        {
                            Reactions = replyReactionMVs,
                            Replies = new List<CommentCreateMV>() // For now, empty; add recursion for deeper nesting if needed
                        });
                    }

                    // Create main comment CommentCreateMV
                    var commentMV = new CommentCreateMV(
                        isEdited: (comment.UpdatedOn != null ? true : false),
                        authorName: comment.User.UserName,
                        authorAvatar: comment.User.ImgPath,
                        commentId: comment.Id,
                        postId: comment.PostId,
                        textContent: comment.Content,
                        imagePath: comment.ImgPath,
                        parentCommentId: comment.ParentCommentId,
                        commenterId: comment.CommenterId,
                        createdAt: comment.CreatedOn
                    )
                    {
                        Reactions = reactionMVs,
                        Replies = replyMVs
                    };

                    sharedPostMV.Comments.Add(commentMV);
                }


                // Load reactions (same logic as regular posts)
                var reactions = await IPRS.GetReactionsByPostAsync(originalPost.Id);
                sharedPostMV.Reactions = reactions.Select(r => new PostReactionMV
                {
                    Id = r.Id,
                    PostId = r.PostId,
                    ReactorId = r.ReactorId,
                    ReactorUserName = r.Reactor?.UserName,
                    Reaction = r.Reaction.ToString(),
                    IsDeleted = r.IsDeleted,
                    CreatedOn = r.CreatedOn
                }).ToList();

                HomePosts.Add(sharedPostMV);
            }



            // Map PostMV
            
            var posts = (await IPS.GetRecentPostsAsync()).ToList();
            foreach (var post in posts)
            {
                
                var postMV = new PostMV
                {
                    CreatedAt = post.CreatedOn,
                    PostUserId = post.UserId,
                    IsEdited = (post.UpdatedOn != null ? true : false),
                    postId = post.Id,
                    PostUserName = post.User.UserName,
                    PostUserTitle = post.User.Title,
                    PostUserImg = post.User.ImgPath ?? "/imgs/Account/default.png",
                    TextContent = post.TextContent,
                    Since = DateTime.Now - post.CreatedOn,
                    Images = new List<string>(),
                    imageCount = await IPIS.GetImageCountForPostAsync(post.Id),
                    CommentsCount = await IPCS.GetCommentCountForPostAsync(post.Id),
                    NumberOfShares = await ISharePS.GetPostShareCountAsync(post.Id),
                    IsSavedByCurrentUser = await ISavePostS.IsPostSavedByUserAsync(post.Id, post.UserId),
                   
        //Comments = new list<>

        ReactionCount = await IPRS.GetReactionCountAsync(post.Id),

                    LikeCount = await IPRS.GetReactionCountAsync(post.Id, ReactionTypes.Like),
                    LoveCount = await IPRS.GetReactionCountAsync(post.Id, ReactionTypes.Love),
                    LaughCount = await IPRS.GetReactionCountAsync(post.Id, ReactionTypes.Haha),
                    SadCount = await IPRS.GetReactionCountAsync(post.Id, ReactionTypes.Sad),
                    AngryCount = await IPRS.GetReactionCountAsync(post.Id, ReactionTypes.Angry),
                    ReactionsNumbers = new List<int>
                    {
                        await IPRS.GetReactionCountAsync(post.Id, ReactionTypes.Like),
                        await IPRS.GetReactionCountAsync(post.Id, ReactionTypes.Love),
                        await IPRS.GetReactionCountAsync(post.Id, ReactionTypes.Haha),
                        await IPRS.GetReactionCountAsync(post.Id, ReactionTypes.Sad),
                        await IPRS.GetReactionCountAsync(post.Id, ReactionTypes.Angry)
                    },
                };

                
                var images = await IPIS.GetImageByPostIdAsync(post.Id);
                foreach (var img in images)
                {
                    postMV.Images.Add(img.ImagePath);
                }

                var comments = await IPCS.GetCommentsForPostAsync(post.Id);
                postMV.Comments = new List<CommentCreateMV>();

                foreach (var comment in comments)
                {
                    // Fetch reactions for this comment
                    var commentReactions = await ICRS.GetReactionsByCommentAsync(comment.Id);

                    var reactionMVs = commentReactions.Select(r => new CommentReactionMV
                    {

                        Id = r.Id,
                        CommentId = r.CommentId,
                        ReactorId = r.ReactorId,
                        ReactorUserName = r.Reactor?.UserName,
                        Reaction = r.Reaction.ToString(),
                        IsDeleted = r.IsDeleted,
                        CreatedOn = r.CreatedOn
                    }).ToList();

                    // Fetch replies for this comment
                    var replies = await IPCS.GetCommentRepliesAsync(comment.Id);

                    var replyMVs = new List<CommentCreateMV>();
                    foreach (var reply in replies)
                    {
                        // Fetch reactions for each reply
                        var replyReactions = await ICRS.GetReactionsByCommentAsync(reply.Id);
                        var replyReactionMVs = replyReactions.Select(r => new CommentReactionMV
                        {
                            Id = r.Id,
                            CommentId = r.CommentId,
                            ReactorId = r.ReactorId,
                            ReactorUserName = r.Reactor?.UserName,
                            Reaction = r.Reaction.ToString(),
                            IsDeleted = r.IsDeleted,
                            CreatedOn = r.CreatedOn
                        }).ToList();

                        // Optionally, recursively fetch nested replies here if you want multi-level threading

                        replyMVs.Add(new CommentCreateMV(
                            isEdited: (comment.CreatedOn != null ? true: false),
                            createdAt: comment.CreatedOn,
                            authorName: comment.User.UserName,
                            authorAvatar: comment.User.ImgPath,
                            commentId: reply.Id,
                            postId: reply.PostId,
                            textContent: reply.Content,
                            imagePath: reply.ImgPath,
                            parentCommentId: reply.ParentCommentId,
                            commenterId: reply.CommenterId
                        )
                        {
                            Reactions = replyReactionMVs,
                            Replies = new List<CommentCreateMV>() // For now, empty; add recursion for deeper nesting if needed
                        });
                    }

                    var commentMV = new CommentCreateMV(
                        isEdited: (comment.UpdatedOn != null ? true:false),
                        authorName: comment.User.UserName,
                        authorAvatar: comment.User.ImgPath,
                        commentId: comment.Id,
                        postId: comment.PostId,
                        textContent: comment.Content,
                        imagePath: comment.ImgPath,
                        parentCommentId: comment.ParentCommentId,
                        commenterId: comment.CommenterId,
                        createdAt: comment.CreatedOn
                        


                    )
                    {
                        Reactions = reactionMVs,
                        Replies = replyMVs
                    };

                    postMV.Comments.Add(commentMV);
                }

                var reactions = await IPRS.GetReactionsByPostAsync(post.Id);

                // Map to PostReactionMV
                postMV.Reactions = reactions.Select(r => new PostReactionMV
                {
                    Id = r.Id,
                    PostId = r.PostId,
                    ReactorId = r.ReactorId,
                    ReactorUserName = r.Reactor?.UserName, // if available
                    Reaction = r.Reaction.ToString(),
                    IsDeleted = r.IsDeleted,
                    CreatedOn = r.CreatedOn
                }).ToList();

                HomePosts.Add(postMV);
            }

            
            



            return View("index", HomePosts);
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

        public async Task <IActionResult> PeopleYouMayKnow()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var peopleYouMayKnow = await _IFS.GetPeopleYouMayKnowAsync(userId);
            var candidates = peopleYouMayKnow.Select(u => u.Id).ToList();
            if (!candidates.Any())
                return View(new List<PoepleMV>());
            var model = new List<PoepleMV>();
            foreach (var f in peopleYouMayKnow)
            {
                var otherUserId = f.Id;
                var mutualCount = await _IFS.GetMutualFriendCountAsync(userId, otherUserId);
                model.Add(new PoepleMV
                {
                    Id = f.Id,
                    Name = f.UserName,
                    ImgPath = f.ImgPath ?? "/imgs/Account/default.png",
                    Title = f.Title,
                    Status = FriendStatus.None, 
                    MutualFriendsCount = mutualCount

                });
            }
            return View(model);
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
            var peopleList = await _IFS.GetFriendsAsync(userId);

            var acceptedRequests = peopleList
                .Where(f => f.Status == FriendStatus.Accepted)
                .ToList();
            var model = new List<PoepleMV>();
            foreach (var f in acceptedRequests) {
                var otherUserId = f.RequesterId == userId ? f.AddresseeId : f.RequesterId;
                var mutualCount = await _IFS.GetMutualFriendCountAsync(userId, otherUserId);
                model.Add(new PoepleMV
                {
                    Id = f.RequesterId == userId ? f.AddresseeId : f.RequesterId,
                    Name = f.RequesterId == userId ? f.Addressee?.UserName : f.Requester?.UserName,
                    ImgPath = f.RequesterId == userId ? f.Addressee?.ImgPath : f.Requester?.ImgPath,
                    Title = f.RequesterId == userId ? f.Addressee?.Title : f.Requester?.Title,
                    Status = f.Status,
                    MutualFriendsCount = mutualCount

                });
            }
            return View(model);

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
