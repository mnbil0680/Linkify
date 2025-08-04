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
            var allFriends = _IFS.GetFriends(id).ToList();
            return View(allFriends);
        }
        [HttpPost]
        public IActionResult AddFriend(string id)
        {
            var requesterId = User.FindFirstValue(ClaimTypes.NameIdentifier); // now i am the requester and iam known from the session
            if (requesterId == id)
            {
                return BadRequest("You cannot send a friend request to yourself.");
            }

            _IFS.AddFriendRequest(requesterId, id);
            return Ok(); //AJAX call success
        }
    }
}
