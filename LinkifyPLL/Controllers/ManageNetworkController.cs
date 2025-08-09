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
        public IActionResult Index()
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var manageNetworkMV = new ManageNetworkMV
                {
                    PendingRequests = _IFS.GetPendingRequests(userId).Select(fr => new ManageUser
                    {
                        UserId = fr.AddresseeId,
                        FullName = fr.Addressee.UserName,
                        AvatarUrl = fr.Addressee.ImgPath,
                        Status = FriendStatus.Pending,
                        Since = fr.RequestDate
                    }).ToList(),
                    AcceptedFriends = _IFS.GetFriends(userId).Select(f => new ManageUser
                    {
                        UserId = f.AddresseeId,
                        FullName = f.Addressee.UserName,
                        AvatarUrl = f.Addressee.ImgPath,
                        Status = FriendStatus.Accepted,
                        Since = f.AcceptanceDate
                    }).ToList(),
                    BlockedUsers = _IFS.GetBlockedUsers(userId).Select(bu => new ManageUser
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
        public IActionResult MyPendingRequests()
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var pendingRequests = _IFS.GetPendingRequests(userId);
                return View(pendingRequests);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        public IActionResult MyPendingRequestsCount()
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var pendingRequestCount = _IFS.GetPendingRequestCount(userId);
                return View(pendingRequestCount);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        public IActionResult MyFriends()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var friends = _IFS.GetFriends(userId);

            return View(friends);
        }
        public IActionResult MyFriendsCount()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var friendsCount = _IFS.GetFriendCount(userId);
            return View(friendsCount);
        }

        public IActionResult MyBlockedUsers()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var blockedUsers = _IFS.GetBlockedUsers(userId);
            return View(blockedUsers);
        }
        public IActionResult MyBlockedUsersCount()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var blockedUsersCount = _IFS.GetBlockedUsers(userId).Count();
            return View(blockedUsersCount);
        }
        
        [HttpPost]
        public IActionResult AcceptRequest(string requesterId)
        {
            try
            {
                var addresseeId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                _IFS.AcceptFriendRequest(requesterId, addresseeId);
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
        public IActionResult DeclineRequest(string requesterId)
        {
            try
            {
                var addresseeId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                _IFS.DeclineFriendRequest(requesterId, addresseeId);
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
        public IActionResult CancelRequest(string addresseeId)
        {
            try
            {
                var requesterId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                _IFS.CancelFriendRequest(requesterId, addresseeId);
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
        public IActionResult BlockUser(string blockedId)
        {
            try
            {
                var blockerId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                _IFS.BlockUser(blockerId, blockedId);
                TempData["SuccessMessage"] = "User blocked successfully.";
            }
            catch (Exception)
            {
                TempData["ErrorMessage"] = "Something went wrong. Please try again.";
            }

            return RedirectToAction("Index");
        }
        [HttpPost]
        public IActionResult UnblockUser(string blockedId)
        {
            try
            {
                var blockerId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                _IFS.UnblockUser(blockerId, blockedId);
                TempData["SuccessMessage"] = "User unblocked successfully.";
            }
            catch (KeyNotFoundException)
            {
                TempData["ErrorMessage"] = "No blocked relationship found.";
            }
            catch (Exception)
            {
                TempData["ErrorMessage"] = "Something went wrong. Please try again.";
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult Unfriend(string userId)
        {
            try
            {
                var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                _IFS.Unfriend(currentUserId, userId);
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
