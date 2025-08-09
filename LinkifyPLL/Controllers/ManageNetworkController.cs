using LinkifyBLL.ModelView;
using LinkifyBLL.Services.Abstraction;
using LinkifyDAL.Enums;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace LinkifyPLL.Controllers
{
    public class ManageNetworkController : Controller
    {
        public readonly IFriendsService _IFS;
        public ManageNetworkController(IFriendsService ifs)
        {
            this._IFS = ifs;
        }
        public async Task<IActionResult> Index()
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var pendingRequests = await _IFS.GetPendingRequestsAsync(userId);
                var acceptedFriends = await _IFS.GetFriendsAsync(userId);
                var blockedUsers = await _IFS.GetBlockedUsersAsync(userId);

                var manageNetworkMV = new ManageNetworkMV
                {
                    PendingRequests = pendingRequests.Select(fr => new ManageUser
                    {
                        UserId = fr.AddresseeId,
                        FullName = fr.Addressee.UserName,
                        AvatarUrl = fr.Addressee.ImgPath,
                        Status = FriendStatus.Pending,
                        Since = fr.RequestDate
                    }).ToList(),
                    AcceptedFriends = acceptedFriends.Select(f => new ManageUser
                    {
                        UserId = f.AddresseeId,
                        FullName = f.Addressee.UserName,
                        AvatarUrl = f.Addressee.ImgPath,
                        Status = FriendStatus.Accepted,
                        Since = f.AcceptanceDate
                    }).ToList(),
                    BlockedUsers = blockedUsers.Select(bu => new ManageUser
                    {
                        UserId = bu.AddresseeId,
                        FullName = bu.Addressee.UserName,
                        AvatarUrl = bu.Addressee.ImgPath,
                        Status = FriendStatus.Blocked,
                        Since = bu.ModificationDate
                    }).ToList()
                };
                return View(manageNetworkMV);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        public async Task <IActionResult> MyPendingRequests()
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var pendingRequests = await _IFS.GetPendingRequestsAsync(userId);
                return View(pendingRequests);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        public async Task<IActionResult> MyPendingRequestsCount()
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var pendingRequestCount = await _IFS.GetPendingRequestCountAsync(userId);
                return View(pendingRequestCount);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        public async Task<IActionResult> MyFriends()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var friends = await _IFS.GetFriendsAsync(userId);

            return View(friends);
        }
        public async Task <IActionResult> MyFriendsCount()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var friendsCount = await _IFS.GetFriendCountAsync(userId);
            return View(friendsCount);
        }

        public async Task<IActionResult> MyBlockedUsers()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var blockedUsers = await _IFS.GetBlockedUsersAsync(userId);
            return View(blockedUsers);
        }
        public async Task<IActionResult> MyBlockedUsersCount()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var blockedUsersCount = await _IFS.GetBlockedUsersAsync(userId);
            var counts= blockedUsersCount.Count();
            return View(counts);
        }

        [HttpPost]
        public async Task<IActionResult> AcceptRequest(string requesterId)
        {
            try
            {
                var addresseeId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                await _IFS.AcceptFriendRequestAsync(requesterId, addresseeId);
                TempData["SuccessMessage"] = "Friend request accepted successfully.";
            }
            catch (KeyNotFoundException ex)
            {
                TempData["ErrorMessage"] = "The friend request no longer exists.";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Something went wrong. Please try again.";
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task <IActionResult> DeclineRequest(string requesterId)
        {
            try
            {
                var addresseeId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                await _IFS.DeclineFriendRequestAsync(requesterId, addresseeId);
                TempData["SuccessMessage"] = "Friend request declined.";
            }
            catch (KeyNotFoundException)
            {
                TempData["ErrorMessage"] = "The friend request no longer exists.";
            }
            catch (Exception)
            {
                TempData["ErrorMessage"] = "Something went wrong. Please try again.";
            }

            return RedirectToAction("Index");
        }
        [HttpPost]
        public async Task <IActionResult> CancelRequest(string addresseeId)
        {
            try
            {
                var requesterId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                await _IFS.CancelFriendRequestAsync(requesterId, addresseeId);
                TempData["SuccessMessage"] = "Friend request cancelled.";
            }
            catch (KeyNotFoundException)
            {
                TempData["ErrorMessage"] = "No pending friend request found.";
            }
            catch (Exception)
            {
                TempData["ErrorMessage"] = "Something went wrong. Please try again.";
            }

            return RedirectToAction("Index");
        }
        [HttpPost]
        public async Task<IActionResult> BlockUser(string blockedId)
        {
            try
            {
                var blockerId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                await _IFS.BlockUserAsync(blockerId, blockedId);
                TempData["SuccessMessage"] = "User blocked successfully.";
            }
            catch (Exception)
            {
                TempData["ErrorMessage"] = "Something went wrong. Please try again.";
            }

            return RedirectToAction("Index");
        }
        [HttpPost]
        //public async Task<IActionResult> UnblockUser(string blockedId)
        //{
        //    try
        //    {
        //        var blockerId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        //        await _IFS.UnblockUserAsync(blockerId, blockedId);
        //        TempData["SuccessMessage"] = "User unblocked successfully.";
        //    }
        //    catch (KeyNotFoundException)
        //    {
        //        TempData["ErrorMessage"] = "No blocked relationship found.";
        //    }
        //    catch (Exception)
        //    {
        //        TempData["ErrorMessage"] = "Something went wrong. Please try again.";
        //    }

        //    return RedirectToAction("Index");
        //}

        [HttpPost]
        public async Task<IActionResult> Unfriend(string userId)
        {
            try
            {
                var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                await _IFS.UnfriendAsync(currentUserId, userId);
                TempData["SuccessMessage"] = "Unfriended successfully.";
            }
            catch (KeyNotFoundException)
            {
                TempData["ErrorMessage"] = "No active friendship found.";
            }
            catch (Exception)
            {
                TempData["ErrorMessage"] = "Something went wrong. Please try again.";
            }

            return RedirectToAction("Index");
        }

    }
}
