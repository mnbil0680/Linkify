using LinkifyBLL.ModelView;
using LinkifyBLL.Services.Abstraction;
using LinkifyDAL.Entities;
using LinkifyDAL.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SempaBLL.Helper;
using System.Security.Claims;


namespace LinkifyPLL.Controllers
{
    public class PostController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IPostService _postService;
        private readonly IPostImagesService _postImageService;
        private readonly IPostReactionsService _postReactionService;
        private readonly IPostCommentsService _postCommentService;
        private readonly ICommentReactionsService _commentReactionService;
        private readonly ISavePostService _savePostService;
        private readonly ISharePostService _sharePostService;
        private readonly UserManager<User> _userManager;

        public PostController(ILogger<HomeController> logger, IPostService postService, IPostImagesService postImageService,
                              IPostReactionsService postReactionService, IPostCommentsService postCommentService, ICommentReactionsService commentReaction,
        UserManager<User> userManager, ISavePostService savePostService, ISharePostService SharePostService)
        {
            _logger = logger;
            _sharePostService = SharePostService;
            _savePostService = savePostService;
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
        public async Task<IActionResult> CreatePost()
        {

            return View("~/Views/Post/CreatePost.cshtml");
        }



        [HttpPost]
        public async Task<IActionResult> CreatePost(PostCreateMV model)
        {
            if (!ModelState.IsValid)
            {
                return View(model); // Return validation errors
            }

            // Validate: At least text or one file must be provided
            if (string.IsNullOrWhiteSpace(model.TextContent) &&
                (model.Images == null || !model.Images.Any()) &&
                (model.Videos == null || !model.Videos.Any()) &&
                (model.PDFs == null || !model.PDFs.Any()))
            {
                ModelState.AddModelError("", "Post must contain text, images, videos, or PDFs.");
                return View(model);
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToAction("Login", "Account");
            }

            // Step 1: Create the post (text content)
            var post = await _postService.CreatePostAsync(user.Id, model.TextContent);

            // Step 2: Upload all files (images, videos, PDFs)
            await UploadFiles(model.Images, "posts/images", post.Id);    // Images folder
            await UploadFiles(model.Videos, "posts/videos", post.Id);   // Videos folder
            await UploadFiles(model.PDFs, "posts/documents", post.Id);  // PDFs folder

            return RedirectToAction("Index", "Home");
        }

        // Helper method to upload files of any type
        private async Task UploadFiles(List<IFormFile> files, string folderName, int postId)
        {
            if (files == null || !files.Any()) return;

            foreach (var file in files)
            {
                if (file.Length == 0) continue;

                string filePath = FileManager.UploadFile(folderName, file);
                if (!filePath.StartsWith("Only ")) // Check if upload succeeded (not an error message)
                {
                    await _postImageService.AddPostImageAsync(new PostImages(filePath, postId));
                }
            }
        }



        [HttpGet]
        public async Task<IActionResult> EditPost(int id)
        {
            var post = await _postService.GetPostByIdAsync(id);
            if (post == null)
                return NotFound();

            var images = await _postImageService.GetImageByPostIdAsync(id);
            var model = new EditPostMV
            {
                PostId = post.Id,
                NewTextContent = post.TextContent,
                ExistingImages = images.Select(img => img.ImagePath).ToList(), // Change this line
                NewImgPaths = null // This will be populated when user uploads new images
            };

            return View(model);
        }



        [HttpPost]
        public async Task<IActionResult> DeletePost(int id)
        {
            await _postService.DeletePostAsync(id);
            // Redirect to Home/Index to reload the page after deletion
            return RedirectToAction("Index", "Home");
        }



        // Should be (correct):
        [HttpPost]
        public async Task<IActionResult> EditPost(EditPostMV model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                // Update the post
                await _postService.UpdatePostAsync(model.PostId, model.NewTextContent);

                // Handle existing images (remove deleted ones)
                var currentImages = await _postImageService.GetImageByPostIdAsync(model.PostId);
                foreach (var image in currentImages)
                {
                    if (!model.ExistingImages.Contains(image.ImagePath))
                    {
                        // Image was removed in UI, delete it
                        // Add method to delete image in IPostImagesService
                        // await _postImageService.DeleteImageAsync(image.Id);
                    }
                }

                // Handle new images
                if (model.NewImgPaths != null && model.NewImgPaths.Any())
                {
                    foreach (var img in model.NewImgPaths)
                    {
                        if (img != null && img.Length > 0)
                        {
                            string imgPath = FileManager.UploadFile("Files", img);
                            var postImage = new PostImages(imgPath, model.PostId);
                            await _postImageService.AddPostImageAsync(postImage);
                        }
                    }
                }

                return RedirectToAction("Index", "Home");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Failed to update post. Please try again.");
                return View(model);
            }
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
                    authorAvatar = user.ImgPath ?? "/imgs/Account/default.png",
                    content = comment.Content,
                    timeAgo = DateTime.Now - comment.CreatedOn,
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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RestorePost([FromBody] RestorePostRequest request)
        {
            try
            {
                if (request == null || request.PostId <= 0)
                {
                    return Json(new { success = false, message = "Invalid request" });
                }

                // Get the current user ID
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userId))
                {
                    return Json(new { success = false, message = "User not authenticated" });
                }

                // Call the service to restore the post
                await _savePostService.RestoreAsync(request.PostId);

                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        public class RestorePostRequest
        {
            public int PostId { get; set; }
        }




        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ArchivePost([FromBody] ArchivePostRequest request)
        {
            try
            {
                if (request == null || request.PostId <= 0)
                {
                    return Json(new { success = false, message = "Invalid request" });
                }

                // Get the current user ID
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userId))
                {
                    return Json(new { success = false, message = "User not authenticated" });
                }

                // Call the service to archive the post
                await _savePostService.ArchiveAsync(request.PostId);

                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        public class ArchivePostRequest
        {
            public int PostId { get; set; }
        }



        [HttpPost]
        public async Task<IActionResult> SavePost([FromBody] SavePostRequest request)
        {
            bool isSaved = await _savePostService.IsPostSavedByUserAsync(request.PostId, request.UserId);

            if (isSaved)
            {
                await _savePostService.ArchiveAsync(request.PostId);
                return Json(new { success = true, isSaved = false });
            }
            else
            {

                await _savePostService.SavePostAsync(request.PostId, request.UserId);
                return Json(new { success = true, isSaved = true });
            }
        }

        public class SavePostRequest
        {
            public string UserId { get; set; }
            public int PostId { get; set; }
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> SavedPost(string userId)
        {

            List<SavePost> Save_Posts = (List<SavePost>)await _savePostService.GetSavedPostsByUserIdAsync(userId);
            List<SavePost> Archive_Posts = (List<SavePost>)await _savePostService.GetArchivedSavedPostsAsync(userId);
            List<SavePost> Posts = Save_Posts.Concat(Archive_Posts).ToList();
            List <SavedPostMV> SavedPosts = new List<SavedPostMV>();
            foreach (var post in Posts)
            {
                if (post == null || post.User == null || post.Post == null)
                    continue; // or handle/log the error as needed

                var SingleSavedPost = new SavedPostMV
                {
                    postId = post.Id,
                    PostUserName = post.User.UserName,
                    PostUserId = post.UserId,
                    PostUserTitle = post.User.Title,
                    PostUserImg = post.User.ImgPath ?? "/imgs/Account/default.png",
                    IsSavedByCurrentUser = await _savePostService.IsPostSavedByUserAsync(post.Id, post.UserId),
                    IsEdited = (post.Post.UpdatedOn != null ? true : false),
                    CreatedAt = post.Post.CreatedOn,
                    IsPremiumUser = false,
                    IsVerified = true,
                    IsArchived = await _savePostService.IsPostArchivedByUserAsync(post.PostId, post.UserId),
                    TextContent = post.Post.TextContent,
                    IsDelted = post.Post.IsDeleted,
                    Since = DateTime.Now - post.Post.CreatedOn,
                    CommentsCount = await _postCommentService.GetCommentCountForPostAsync(post.Id),
                    NumberOfShares = await _sharePostService.GetPostShareCountAsync(post.Id),



                    LikeCount = await _postReactionService.GetReactionCountAsync(post.Id, ReactionTypes.Like),
                    LoveCount = await _postReactionService.GetReactionCountAsync(post.Id, ReactionTypes.Love),
                    LaughCount = await _postReactionService.GetReactionCountAsync(post.Id, ReactionTypes.Haha),
                    SadCount = await _postReactionService.GetReactionCountAsync(post.Id, ReactionTypes.Sad),
                    AngryCount = await _postReactionService.GetReactionCountAsync(post.Id, ReactionTypes.Angry),
                    ReactionCount = await _postReactionService.GetReactionCountAsync(post.Id),
                    imageCount = await _postImageService.GetImageCountForPostAsync(post.Id)

                };

                // imgs
                SingleSavedPost.Images = new List<string>();

                // Get images for this post
                var gettingImgs = await _postImageService.GetImageByPostIdAsync(post.PostId);
                foreach (var img in gettingImgs)
                {
                    SingleSavedPost.Images.Add(img.ImagePath);
                }


                // Comments
                // Initialize the Comments property in SingleSavedPost
                SingleSavedPost.Comments = new List<CommentCreateMV>();

                // Get and map comments
                var gettingComments = await _postCommentService.GetCommentsForPostAsync(post.PostId);
                foreach (var comment in gettingComments)
                {
                    if (comment != null)
                    {
                        var commentCreate = new CommentCreateMV(
                             isEdited: comment.UpdatedOn != null,
                             createdAt: comment.CreatedOn,
                             authorName: comment.User?.UserName ?? "",
                             authorAvatar: comment.User?.ImgPath ?? "~/imgs/Account/default.png",
                             commentId: comment.Id,
                             postId: comment.PostId,
                             textContent: comment.Content,
                             imagePath: comment.ImgPath,
                             parentCommentId: comment.ParentCommentId,
                             commenterId: comment.CommenterId
                        );

                        // Add directly to the SingleSavedPost.Comments collection
                        SingleSavedPost.Comments.Add(commentCreate);
                    }
                }

                // Update comment count based on the actual comments added
                SingleSavedPost.CommentsCount = SingleSavedPost.Comments.Count;



                // Initialize the Reactions property in SingleSavedPost
                SingleSavedPost.Reactions = new List<PostReactionMV>();

                // Get and map reactions
                var gettingPostReactions = await _postReactionService.GetReactionsByPostAsync(post.PostId);
                foreach (var reaction in gettingPostReactions)
                {
                    if (reaction != null)
                    {
                        var postReactionMV = new PostReactionMV
                        {
                            Id = reaction.Id,
                            PostId = reaction.PostId,
                            ReactorId = reaction.ReactorId,
                            ReactorUserName = reaction.Reactor?.UserName, // null-safe
                            Reaction = reaction.Reaction.ToString(),
                            IsDeleted = reaction.IsDeleted,
                            CreatedOn = reaction.CreatedOn
                        };

                        // Add directly to the SingleSavedPost.Reactions collection
                        SingleSavedPost.Reactions.Add(postReactionMV);
                    }
                }





                SavedPosts.Add(SingleSavedPost);




            }
            return View(SavedPosts);
            //return View("~/Views/Post/SavedPost2.cshtml", SavedPosts);
        }




        [HttpPost("api/comments/{commentId}/reaction")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleCommentReaction(int commentId, [FromBody] ToggleCommentReactionDto model)
        {
            try
            {
                // Override model's CommentId with route value to avoid mismatches
                model.CommentId = commentId;

                string input = model.ReactionType; // could be "😂", "haha", etc.
                ReactionTypes reactionType;

                if (input == "😂" || input.Equals("Haha", StringComparison.OrdinalIgnoreCase))
                    reactionType = ReactionTypes.Haha;
                else if (input == "👍" || input.Equals("Like", StringComparison.OrdinalIgnoreCase))
                    reactionType = ReactionTypes.Like;
                else if (input == "❤️" || input.Equals("Love", StringComparison.OrdinalIgnoreCase))
                    reactionType = ReactionTypes.Love;
                else if (input == "😢" || input.Equals("Sad", StringComparison.OrdinalIgnoreCase))
                    reactionType = ReactionTypes.Sad;
                else if (input == "😡" || input.Equals("Angry", StringComparison.OrdinalIgnoreCase))
                    reactionType = ReactionTypes.Angry;
                else
                    return BadRequest(new { success = false, message = "Invalid reaction type" });

                // Get all current reactions for this comment
                List<CommentReactions> existingReactions = (List<CommentReactions>)await _commentReactionService.GetReactionsByCommentAsync(commentId);

                // Check if this exact reaction (same user, same type, same comment) already exists
                bool reactionExists = existingReactions.Any(r =>
                    r.ReactorId == model.UserId &&
                    r.CommentId == model.CommentId &&
                    r.Reaction == reactionType &&
                    !r.IsDeleted);

                // Only toggle if this exact reaction doesn't already exist
                if (!reactionExists)
                {
                    await _commentReactionService.ToggleReactionAsync(
                        model.CommentId,
                        model.UserId,
                        reactionType
                    );
                }



                var reactions = await _commentReactionService.GetReactionsByCommentAsync(commentId);
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
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    success = false,
                    message = "An error occurred while processing your comment reaction"
                });
            }
        }

        // Add this class for the request model
        public class ToggleCommentReactionDto
        {
            public int CommentId { get; set; }
            public string UserId { get; set; }
            public string ReactionType { get; set; }
        }


    }

}   