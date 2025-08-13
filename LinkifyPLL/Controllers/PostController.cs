using LinkifyBLL.ModelView;
using LinkifyBLL.Services.Abstraction;
using LinkifyBLL.Services.Implementation;
using LinkifyDAL.Entities;
using LinkifyDAL.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using SempaBLL.Helper;
using static System.Net.Mime.MediaTypeNames;


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

            return View("~/Views/Post/CreatePost.cshtml");     
        }


        
        [HttpPost]
        public async Task<IActionResult> CreatePost(PostCreateMV model)
        {
            if (!ModelState.IsValid)
            {
                return View(model); // Return to form with validation errors
            }

            // Check if content is provided (since Images are optional)
            if (string.IsNullOrWhiteSpace(model.TextContent) && (model.Images == null))
            {
                ModelState.AddModelError("", "Post Must Have Text Or Images or Both .");
                return View(model);
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToAction("Login", "Account");
            }

            
               var post =   await _postService.CreatePostAsync(user.Id, model.TextContent);
            


            // Get current user
            


            if (model.Images != null && model.Images.Any())
            {
                foreach (var img in model.Images)
                {
                    if (img != null && img.Length > 0)
                    {
                        string imgPath =  FileManager.UploadFile("Files", img);
                        PostImages imgy = new PostImages(imgPath, post.Id);
                        await _postImageService.AddPostImageAsync(imgy);

                    }
                }
            }

            // Save post to database
            await _postService.CreatePostAsync(user.Id, model.TextContent);

            // Redirect to a success page or another action
            return RedirectToAction("Index" , "Home");







        }



        [HttpGet]
        public async Task<IActionResult> EditPost(PostCreateMV model)
        {
            
        }

        [HttpPost]
        public async Task<IActionResult> EditPost(PostCreateMV model)
        {

        }



        [HttpPost("api/posts/{postId}/comments")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateCommentApi(int postId, [FromBody] CreateCommentRequest request)
        {
            try
            {
                if (string.IsNullOrEmpty(request.Content))
                {
                    return Json(new { success = false, message = "Comment content is required" });
                }

                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                {
                    return Json(new { success = false, message = "User not authenticated" });
                }

                // Create the comment
                var comment = await _postCommentService.CreateCommentAsync(
                    postId,
                    user.Id,
                    request.Content
                );

                // Return the created comment data
                var commentData = new
                {
                    id = comment.Id,
                    authorName = user.UserName,
                    authorAvatar = user.ImgPath ?? "/images/default-avatar.png",
                    content = comment.Content,
                    timeAgo = "now",
                    reactions = new List<object>()
                };

                return Json(new
                {
                    success = true,
                    comment = commentData,
                    message = "Comment posted successfully"
                });
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    success = false,
                    message = "Failed to post comment"
                });
            }
        }

        // Add this class for the request model
        public class CreateCommentRequest
        {
            public string Content { get; set; }
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

        public class ToggleReactionDto
        {
            public int PostId { get; set; }
            public string UserId { get; set; }
            public string ReactionType { get; set; }
        }


        

        [HttpPost("api/posts/{postId}/reaction")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleReaction(int postId, [FromBody] ToggleReactionDto model)
        {
            try
            {
                // Override model's PostId with route value to avoid mismatches
                model.PostId = postId;

                await _postReactionService.ToggleReactionAsync(
                    model.PostId,
                    model.UserId,
                    Enum.Parse<ReactionTypes>(model.ReactionType)
                );

                var reactions = await _postReactionService.GetReactionsByPostAsync(postId);
                var activeReactions = reactions.Where(r => !r.IsDeleted).ToList();

                return Ok(new
                {
                    success = true,
                    userReaction = activeReactions.FirstOrDefault(r => r.ReactorId == model.UserId)?.Reaction.ToString(),
                    reactionCount = activeReactions.Count,
                    topReactions = activeReactions
                        .GroupBy(r => r.Reaction)
                        .OrderByDescending(g => g.Count())
                        .Take(3)
                        .Select(g => g.Key.ToString())
                        .ToList()
                });
            }
            catch
            {
                return BadRequest(new
                {
                    success = false,
                    message = "An error occurred while processing your reaction"
                });
            }
        }


        [HttpPost]
        [Authorize] // Ensure this action requires authentication
        [ValidateAntiForgeryToken] // Add this to validate the anti-forgery token
        public async Task<IActionResult> ToggleReaction([FromBody] ToggleReactionRequest request)
        {
            try
            {
                // Log the incoming request
                Console.WriteLine($"ToggleReaction called: PostId={request.PostId}, UserId={request.UserId}, ReactionType={request.ReactionType}");

                // Validate input
                if (string.IsNullOrEmpty(request.UserId))
                {
                    return Json(new { success = false, message = "User ID is required" });
                }

                if (request.PostId <= 0)
                {
                    return Json(new { success = false, message = "Invalid post ID" });
                }

                // Check if user is authenticated
                if (!User.Identity.IsAuthenticated)
                {
                    return Json(new { success = false, message = "Authentication required" });
                }

                // Validate that the provided userId matches the authenticated user
                var currentUserId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                if (currentUserId != request.UserId)
                {
                    return Json(new { success = false, message = "User ID mismatch" });
                }

                // Parse reaction type if provided
                ReactionTypes? reactionEnum = null;
                if (!string.IsNullOrEmpty(request.ReactionType))
                {
                    if (!Enum.TryParse<ReactionTypes>(request.ReactionType, true, out var parsedReaction))
                    {
                        return Json(new { success = false, message = $"Invalid reaction type: {request.ReactionType}" });
                    }
                    reactionEnum = parsedReaction;
                }

                // Toggle the reaction
                if (reactionEnum.HasValue)
                {
                    await _postReactionService.ToggleReactionAsync(request.PostId, request.UserId, reactionEnum.Value);
                }
                else
                {
                    // Remove reaction logic if your service supports it
                    // await _postReactionService.RemoveReactionAsync(request.PostId, request.UserId);
                }

                // Get updated counts
                var reactions = await _postReactionService.GetReactionsByPostAsync(request.PostId);
                var reactionCount = reactions.Count(r => !r.IsDeleted);

                return Json(new
                {
                    success = true,
                    message = "Reaction updated successfully",
                    reactionCount = reactionCount,
                    topReactions = reactions
                        .Where(r => !r.IsDeleted)
                        .GroupBy(r => r.Reaction)
                        .OrderByDescending(g => g.Count())
                        .Take(3)
                        .Select(g => g.Key.ToString().ToLower())
                        .ToArray()
                });
            }
            catch (Exception ex)
            {
                // Log the full exception
                Console.WriteLine($"Error in ToggleReaction: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");

                return Json(new
                {
                    success = false,
                    message = "An error occurred while processing your reaction",
                    error = ex.Message // Remove this in production
                });
            }
        }

        // Add this class to represent the request model
        public class ToggleReactionRequest
        {
            public int PostId { get; set; }
            public string UserId { get; set; }
            public string ReactionType { get; set; }
        }



    }

}
