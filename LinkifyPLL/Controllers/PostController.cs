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
        private readonly UserManager<User> _userManager;
        public PostController(IPostService postService, IPostImagesService postImageService, 
                              IPostReactionsService postReactionService, IPostCommentsService postCommentService,
        UserManager<User> userManager)
        {
            _postService = postService;
            _postImageService = postImageService;
            _postReactionService = postReactionService;
            _postCommentService = postCommentService;
            _userManager = userManager;
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


        [HttpPost]
        public async Task<IActionResult> ToggleReaction(int postId, string userId, string reactionType)
        {


            await _postReactionService.ToggleReactionAsync(postId, userId, Enum.Parse<ReactionTypes>(reactionType));
            return RedirectToAction("Index", "Home");
        }

    }
}
