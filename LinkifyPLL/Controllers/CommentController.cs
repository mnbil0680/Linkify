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

        [HttpGet]
        public async Task<IActionResult> Reply(int? parentCommentId, int? postId, string commenterId)
        {
            // Prepare a model for the reply form (optional, for prefilling fields)
            var model = new CommentCreateMV(
                commentId: 0, // New reply, so no ID yet
                postId: postId ?? 0,
                textContent: "",
                imagePath: null,
                parentCommentId: parentCommentId,
                commenterId: commenterId
            );
            return View("~/Views/Comment/Reply.cshtml", model);
        }

        [HttpPost]
        public async Task<IActionResult> Reply(string ReplyText, int PostId, int ParentCommentId, string CommenterId, string? imgAdded = null)
        {
            // Create the reply using the service
            await IPCS.ReplyToCommentAsync(ParentCommentId, CommenterId, ReplyText, imgAdded);
            return RedirectToAction("Index", "Home");
        }


        [HttpGet]
        public async Task<IActionResult> EditReply(int ReplyId, string OldText, int PostId, string CommenterId, string OldImgPath = null, int? ParentCommentId = null)
        {
            // Prepare the model for the edit form
            var reply = new CommentCreateMV(ReplyId, PostId, OldText, OldImgPath, ParentCommentId, CommenterId);
            return View("~/Views/Comment/EditReply.cshtml", reply);
        }

        [HttpPost]
        public async Task<IActionResult> EditReply(int ReplyId, string newText, string imgAdded = null)
        {
            // Update the reply using the service
            await IPCS.UpdateCommentAsync(ReplyId, newText, imgAdded);
            return RedirectToAction("Index", "Home");
        }
        [HttpPost]
        public async Task<IActionResult> DeleteReply(int ReplyId)
        {
            await IPCS.DeleteCommentAsync(ReplyId);
            return RedirectToAction("Index", "Home");
        }


    }
}
