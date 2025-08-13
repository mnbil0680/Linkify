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
                createdAt: DateTime.Now,
                isEdited: false,
                authorName: "comment.User.UserName",
                authorAvatar: "comment.User.ImgPath",
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

        ////////////////////////////////
        ///

        [HttpPut]
        [Route("api/comments/{commentId}/edit")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditCommentApi(int commentId, [FromBody] EditCommentRequest request)
        {
            if (request == null || string.IsNullOrWhiteSpace(request.Content))
                return Json(new { success = false, message = "Comment cannot be empty." });

            // Optionally: Check if the current user is the owner
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var isOwner = await IPCS.IsCommentOwnerAsync(commentId, userId);
            if (!isOwner)
                return Json(new { success = false, message = "You are not authorized to edit this comment." });

            try
            {
                await IPCS.UpdateCommentAsync(commentId, request.Content, null);

                
                // Optionally, fetch the updated comment for UI update
                var updatedComment = await IPCS.GetCommentAsync(commentId);

                return Json(new
                {
                    success = true,
                    content = updatedComment.Content,
                    updatedOn = updatedComment.UpdatedOn
                });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Failed to update comment." });
            }
        }

        public class EditCommentRequest
        {
            public string Content { get; set; }
        }

        [HttpDelete]
        [Route("api/comments/{commentId}/delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteCommentApi(int commentId)
        {
            // Optionally: Check if the current user is the owner
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var isOwner = await IPCS.IsCommentOwnerAsync(commentId, userId);
            if (!isOwner)
                return Json(new { success = false, message = "You are not authorized to delete this comment." });

            try
            {
                await IPCS.DeleteCommentAsync(commentId);
                return Json(new { success = true });
            }
            catch
            {
                return Json(new { success = false, message = "Failed to delete comment." });
            }
        }


        [HttpPost]
        [Route("api/comments/reply")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateReply([FromBody] CreateReplyRequest request)
        {
            if (request == null || string.IsNullOrWhiteSpace(request.Content))
                return Json(new { success = false, message = "Reply cannot be empty." });

            // Get current user ID
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
                return Json(new { success = false, message = "User not authenticated." });

            try
            {
                // Create the reply using the service
                var reply = await IPCS.ReplyToCommentAsync(request.ParentCommentId, userId, request.Content);

                // Map to CommentCreateMV for the response
                var replyVm = MapToCommentCreateMV(reply);

                return Json(new
                {
                    success = true,
                    reply = new
                    {
                        id = replyVm.CommentID,
                        authorName = replyVm.AuthorName,
                        authorAvatar = replyVm.AuthorAvatar,
                        content = replyVm.TextContent,
                        isEdited = replyVm.IsEdited,
                        createdAt = replyVm.CreatedAt,
                        since = replyVm.Since,
                        timeAgo = FormatSince(replyVm.Since)
                    }
                });
            }
            catch (Exception)
            {
                return Json(new { success = false, message = "Failed to post reply." });
            }
        }

        // Request DTO
        public class CreateReplyRequest
        {
            public int ParentCommentId { get; set; }
            public string Content { get; set; }
        }

        // Helper: Map PostComments to CommentCreateMV
        private CommentCreateMV MapToCommentCreateMV(PostComments comment)
        {
            if (comment == null) return null;

            return new CommentCreateMV(
                isEdited: comment.UpdatedOn != null,
                createdAt: comment.CreatedOn,
                authorName: comment.User?.UserName ?? "",
                authorAvatar: comment.User?.ImgPath ?? "/imgs/Account/default.png",
                commentId: comment.Id,
                postId: comment.PostId,
                textContent: comment.Content,
                imagePath: comment.ImgPath,
                parentCommentId: comment.ParentCommentId,
                commenterId: comment.CommenterId
            )
            {
                Since = DateTime.UtcNow - comment.CreatedOn
            };
        }

        // Helper: Format time since (matches your Razor function)
        private string FormatSince(TimeSpan since)
        {
            if (since.TotalMinutes < 60)
                return $"{since.TotalMinutes:0} min ago";
            if (since.TotalHours < 24)
                return $"{since.TotalHours:0} hour{(since.TotalHours >= 2 ? "s" : "")} ago";
            if (since.TotalDays < 30)
                return $"{since.TotalDays:0} day{(since.TotalDays >= 2 ? "s" : "")} ago";
            return $"{(since.TotalDays / 30):0} month{((since.TotalDays / 30) >= 2 ? "s" : "")} ago";
        }

        [HttpPut]
        [Route("api/replies/{replyId}/edit")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditReply(int replyId, [FromBody] EditReplyRequest request)
        {
            if (request == null || string.IsNullOrWhiteSpace(request.Content))
                return Json(new { success = false, message = "Reply cannot be empty." });

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
                return Json(new { success = false, message = "User not authenticated." });

            // Check ownership
            var isOwner = await IPCS.IsCommentOwnerAsync(replyId, userId);
            if (!isOwner)
                return Json(new { success = false, message = "Unauthorized." });

            try
            {
                await IPCS.UpdateCommentAsync(replyId, request.Content);
                return Json(new { success = true });
            }
            catch (Exception)
            {
                return Json(new { success = false, message = "Failed to update reply." });
            }
        }

        public class EditReplyRequest
        {
            public string Content { get; set; }
        }



        [HttpDelete]
        [Route("api/replies/{replyId}/delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteReply(int replyId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
                return Json(new { success = false, message = "User not authenticated." });

            // Check ownership
            var isOwner = await IPCS.IsCommentOwnerAsync(replyId, userId);
            if (!isOwner)
                return Json(new { success = false, message = "Unauthorized." });

            try
            {
                await IPCS.DeleteCommentAsync(replyId);
                return Json(new { success = true });
            }
            catch (Exception)
            {
                return Json(new { success = false, message = "Failed to delete reply." });
            }
        }


    }
}
