using LinkifyBLL.ModelView;
using LinkifyBLL.Services.Abstraction;
using LinkifyBLL.Services.Implementation;
using LinkifyDAL.Entities;
using LinkifyDAL.Enums;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SempaBLL.Helper;


namespace LinkifyPLL.Controllers
{
    public class PostController : Controller
    {
        private readonly IPostService _postService;
        private readonly IPostImagesService _postImageService;
        private readonly IPostReactionsService _postReactionService;
        private readonly IPostCommentsService _postCommentService;
        private readonly ICommentReactionsService _commentReactionService;
        private readonly UserManager<User> _userManager;

        public PostController(IPostService postService, IPostImagesService postImageService, 
                              IPostReactionsService postReactionService, IPostCommentsService postCommentService, ICommentReactionsService commentReaction,
        UserManager<User> userManager)
        {
            _postService = postService;
            _postImageService = postImageService;
            _postReactionService = postReactionService;
            _postCommentService = postCommentService;
            _userManager = userManager;
            _commentReactionService = commentReaction;
        }
        public IActionResult Index()
        {
            return View();
        }
        [HttpGet]
        public async Task <IActionResult> CreatePost()
        {

            return View();     
        }

        [HttpPost]
        public async Task<IActionResult> CreatePost(PostCreateMV model)
        {
            if (!ModelState.IsValid)
                return View(model); // Show validation errors

            var imageFileNames = new List<string>();
            if (model.Images != null && model.Images.Count > 0)
            {
                foreach (var file in model.Images)
                {
                    var fileName = FileManager.UploadFile("Files", file);
                    imageFileNames.Add(fileName);
                }
            }

            var user = await _userManager.GetUserAsync(User);
            // You need to save imageFileNames with the post!
            var post = await _postService.CreatePostAsync(user.Id, model.TextContent);

            // Save images to the post (if you have a service for that)
            if (imageFileNames.Count > 0)
            {
                foreach (var img in imageFileNames)
                {
                    await _postImageService.AddPostImageAsync(new PostImages(img, post.Id));
                }
            }

            return RedirectToAction("Index", "Home");
        }


        [HttpPost]
        public async Task<IActionResult> CreateComment(CommentCreateMV model)
        {
            if (!ModelState.IsValid)
                return RedirectToAction("Index", "Home");
            var user = await _userManager.GetUserAsync(User);
            await _postCommentService.CreateCommentAsync(model.PostId,user.Id, model.TextContent,model.ImagePath, model.ParentCommentId);
            return RedirectToAction("Home");
        }


        [HttpGet("api/posts/{postId}/comments")]
        public async Task<IActionResult> GetComments(int postId, int skip = 0, int take = 5)
        {
            try
            {
                // Get comments from database
                var allComments = await _postCommentService.GetCommentsForPostAsync(postId);
                var comments = allComments.Skip(skip).Take(take).ToList();

                // Check if there are more comments
                var hasMore = allComments.Count() > (skip + take);

                // Map to view models
                var commentMVs = new List<object>();

                foreach (var comment in comments)
                {
                    // Get reactions for this comment
                    var commentReactions = await _commentReactionService.GetReactionsByCommentAsync(comment.Id);

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

                    commentMVs.Add(new
                    {
                        id = comment.Id,
                        authorName = comment.User.UserName,
                        authorAvatar = comment.User.ImgPath ?? "/images/default-avatar.png",
                        content = comment.Content,
                        timeAgo = FormatTimeAgo(DateTime.Now - comment.CreatedOn),
                        reactions = reactionMVs
                    });
                }

                return Json(new
                {
                    success = true,
                    comments = commentMVs,
                    hasMore = hasMore
                });
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    success = false,
                    message = "Failed to load comments"
                });
            }
        }

        // Helper method for time formatting
        private string FormatTimeAgo(TimeSpan since)
        {
            if (since.TotalMinutes < 60)
                return $"{since.TotalMinutes:0} min ago";
            if (since.TotalHours < 24)
                return $"{since.TotalHours:0} hour{(since.TotalHours >= 2 ? "s" : "")} ago";
            if (since.TotalDays < 30)
                return $"{since.TotalDays:0} day{(since.TotalDays >= 2 ? "s" : "")} ago";
            return $"{(since.TotalDays / 30):0} month{(since.TotalDays / 30 >= 2 ? "s" : "")} ago";
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleReaction(int postId, string userId, string reactionType)
        {
            if (string.IsNullOrEmpty(reactionType))
            {
                // Remove reaction
                
            }
            else
            {
                await _postReactionService.ToggleReactionAsync(
                    postId, userId, Enum.Parse<ReactionTypes>(reactionType)
                );
            }

            // Get updated counts
            var reactions = await _postReactionService.GetReactionsByPostAsync(postId);
            var reactionCount = reactions.Count();
            var topReactions = reactions
                .GroupBy(r => r.Reaction)
                .OrderByDescending(g => g.Count())
                .Take(3)
                .Select(g => g.Key.ToString())
                .ToList();

            return Json(new
            {
                success = true,
                userReaction = reactionType,
                reactionCount,
                topReactions
            });
        }

    }

}
