using LinkifyBLL.Services.Abstraction;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace LinkifyPLL.Controllers
{
    public class FriendsController : Controller
    {
        public readonly IFriendsService _IFS;
        public FriendsController(IFriendsService ifs)
        {
            this._IFS = ifs;
        }
        public IActionResult Index()
        {
            string id = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var allFriends = _IFS.GetFriendsAsync(id).Result.ToList();
            return View(allFriends);
        }

        [HttpPost]
        public async Task<IActionResult> AddFriend(string id)
        {
            var requesterId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (requesterId == id)
                return BadRequest("You cannot send a friend request to yourself.");

            await _IFS.AddFriendRequestAsync(requesterId, id);
            return Ok(new { message = "Request Sent" });
        }


    }
}
