using LinkifyBLL.Services.Abstraction;
using Microsoft.AspNetCore.Mvc;

namespace LinkifyPLL.Controllers
{
    public class FriendsController : Controller
    {
        public readonly IFriendsService _IFS;
        public FriendsController(IFriendsService ifs)
        {
            this._IFS = ifs;
        }
        public IActionResult Index(string id)
        {
            var allFriends = _IFS.GetFriends(id).ToList();
            return View(allFriends);
        }
        [HttpPost]
        public IActionResult AddFriend(string id1, string id2)
        {
            _IFS.AddFriendRequest(id1, id2);
            return RedirectToAction("Index");
        }
    }
}
