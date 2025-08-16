using LinkifyBLL.Services.Abstraction;
using Microsoft.AspNetCore.Mvc;

namespace LinkifyPLL.Controllers
{
    public class ShareController : Controller
    {
        private readonly ISharePostService _sharePostService;

        public ShareController(ISharePostService sharePostService)
        {
            _sharePostService = sharePostService;
        }

        public class CreateShareRequest
        {
            public int PostId { get; set; }
            public string UserId { get; set; }
            public string Caption { get; set; }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> CreateShare([FromBody] CreateShareRequest request)
        {
            try
            {
                var hasShared = await _sharePostService.HasUserSharedPostAsync(request.PostId, request.UserId);
                if (hasShared)
                {
                    return Json(new { success = false, message = "Post already shared" });
                }

                var share = await _sharePostService.SharePostAsync(
                    request.PostId,
                    request.UserId,
                    string.IsNullOrEmpty(request.Caption) ? null : request.Caption
                );

                return Json(new
                {
                    success = true,
                    message = "Post shared successfully",
                    shareId = share.Id,
                    postId = share.PostId
                });
            }
            catch
            {
                return Json(new { success = false, message = "Failed to share post" });
            }
        }


        public class CheckIfSharedRequest
        {
            public int PostId { get; set; }
            public string UserId { get; set; }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> CheckIfShared([FromBody] CheckIfSharedRequest request)
        {
            try
            {
                var hasShared = await _sharePostService.HasUserSharedPostAsync(request.PostId, request.UserId);

                if (hasShared)
                {
                    var userShare = await _sharePostService.GetUserShareOfPostAsync(request.PostId, request.UserId);
                    return Json(new { hasShared = true, shareId = userShare.Id });
                }

                return Json(new { hasShared = false });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error checking share status" });
            }
        }



        [HttpPost]
        public async Task<JsonResult> UpdateCaption(int shareId, string newCaption)
        {
            try
            {
                await _sharePostService.UpdateShareCaptionAsync(shareId, newCaption);
                return Json(new { success = true, message = "Caption updated successfully" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Failed to update caption" });
            }
        }


        [HttpPost]
        public async Task<JsonResult> GetShareCount(int postId)
        {
            try
            {
                var count = await _sharePostService.GetPostShareCountAsync(postId);
                return Json(new { success = true, count = count });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Failed to get share count" });
            }
        }

        [HttpPost]
        public async Task<JsonResult> ArchiveShare(int shareId)
        {
            try
            {
                await _sharePostService.ArchiveShareAsync(shareId);
                return Json(new { success = true, message = "Share archived successfully" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Failed to archive share" });
            }
        }

        [HttpPost]
        public async Task<JsonResult> RestoreShare(int shareId)
        {
            try
            {
                await _sharePostService.RestoreShareAsync(shareId);
                return Json(new { success = true, message = "Share restored successfully" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Failed to restore share" });
            }
        }

























        // View actions for displaying share data
        public async Task<IActionResult> UserShares(string userId, bool includeArchived = false)
        {
            try
            {
                var shares = await _sharePostService.GetUserSharesAsync(userId, includeArchived);
                return View(shares);
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Failed to load user shares";
                return RedirectToAction("Index", "Home");
            }
        }

        public async Task<IActionResult> PostShares(int postId, bool includeArchived = false)
        {
            try
            {
                var shares = await _sharePostService.GetPostSharesAsync(postId, includeArchived);
                ViewBag.PostId = postId;
                return View(shares);
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Failed to load post shares";
                return RedirectToAction("Index", "Home");
            }
        }

        public async Task<IActionResult> ArchivedShares(string userId)
        {
            try
            {
                var shares = await _sharePostService.GetUserArchivedSharesAsync(userId);
                return View(shares);
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Failed to load archived shares";
                return RedirectToAction("Index", "Home");
            }
        }

        [HttpPost]
        public async Task<JsonResult> GetUserShareCount(string userId, bool includeArchived = false)
        {
            try
            {
                var count = await _sharePostService.GetUserShareCountAsync(userId, includeArchived);
                return Json(new { success = true, count = count });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Failed to get user share count" });
            }
        }

        [HttpPost]
        public async Task<JsonResult> GetShareById(int shareId)
        {
            try
            {
                var share = await _sharePostService.GetShareByIdAsync(shareId);
                return Json(new { success = true, share = share });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Failed to get share details" });
            }
        }



    }
}
