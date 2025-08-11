using LinkifyBLL.ModelView;
using LinkifyBLL.Services.Abstraction;
using LinkifyBLL.Services.Implementation;
using LinkifyDAL.Entities;
using LinkifyDAL.Enums;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace LinkifyPLL.Controllers
{
    public class CommentController : Controller
    {
        public readonly IPostCommentsService IPCS;
        private readonly ICommentReactionsService ICRS;

        public CommentController(IPostCommentsService ipcs, ICommentReactionsService icrs)
        {

           
            this.IPCS = ipcs;
            this.ICRS = icrs;


        }

        public async Task<IActionResult> DeleteComment(int CommentID, string newText, string imgAdded = null)
        {
            await IPCS.DeleteCommentAsync(CommentID);
            return RedirectToAction("Index","Home");
        }

        [HttpGet]
        public async Task<IActionResult> EditComment(int CommentId, string OldText, int PostId, string CommenterId, string OldImgPath = null, int? ParentCommentId = null)
        {
            CommentCreateMV oldComment = new CommentCreateMV(CommentId, PostId, OldText, OldImgPath, ParentCommentId, CommenterId);
            return View("~/Views/Home/testEditComment.cshtml", oldComment);
        }

        [HttpPost]
        public async Task<IActionResult> EditComment(int CommentID, string newText, string imgAdded = null)
        {
            await IPCS.UpdateCommentAsync(CommentID, newText, imgAdded);
            return RedirectToAction("Index","Home");
        }

        [HttpGet]
        public async Task<IActionResult> TestComment()
        {
            return View("~/Views/Home/testcomment.cshtml");
        }

        [HttpPost]
        public async Task<IActionResult> TestComment(string CommentText, string CommenterId, string PostId)
        {

            await IPCS.CreateCommentAsync(int.Parse(PostId), CommenterId, CommentText);


            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        public async Task<IActionResult> ToggleReaction(int commentId,string  userId, string reactionType)
        {

           
            await ICRS.ToggleReactionAsync(commentId, userId, Enum.Parse<ReactionTypes>(reactionType));
            return RedirectToAction("Index","Home");
        }
    }
}
