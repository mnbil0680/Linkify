using LinkifyBLL.ModelView;
using LinkifyBLL.Services.Abstraction;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace LinkifyPLL.Controllers
{
    public class ContactController : Controller
    {
        private readonly IContactService _contactService;
        private readonly UserManager<UserMV> _userManager;
        public ContactController(IContactService contactService, UserManager<UserMV> userManager)
        {
            _contactService = contactService;
            _userManager = userManager;
        }
        public async Task <IActionResult> Info()
        {
            // who is the current user?
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound("User not found");
            }
            var contacts = _contactService.GetContactsByUserId(user.Id);
            var model = new ContactVM
            {
                MainEmail = user.Email,
                Contacts = contacts // might be empty, that's okay
            };
            return View(model);
        }
        public IActionResult DeleteContact(int id)
        {
            _contactService.DeleteContact(id);
            return RedirectToAction("ContactInfo");
        }

        [HttpPost]
        public Task<IActionResult> AddContact()
        {

        }

    }
}
