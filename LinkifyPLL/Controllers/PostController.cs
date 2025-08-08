using LinkifyBLL.ModelView;
using LinkifyBLL.Services.Abstraction;
using LinkifyBLL.Services.Implementation;
using LinkifyDAL.Entities;
using LinkifyDAL.Enums;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;


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
        [HttpPost]
        public async Task <IActionResult> CreatePost(PostCreateMV model)
        {
            if (!ModelState.IsValid)
                return RedirectToAction("Index", "Home");

            var user = await _userManager.GetUserAsync(User);
            //await _postService.CreatePostAsync(user.Id, model.TextContent,model.Images );
            await _postService.CreatePostAsync(user.Id, model.TextContent);
            return RedirectToAction("Home");
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
        public async Task<IActionResult> CreateReaction(int postId, ReactionTypes reaction)
        {
            if (!ModelState.IsValid)
                return RedirectToAction("Index", "Home");
            var user = await _userManager.GetUserAsync(User);
            if(user == null)
                return Unauthorized();
            await _postReactionService.ToggleReactionAsync(postId, user.Id, reaction);
            return Ok();
        }
    }
}
