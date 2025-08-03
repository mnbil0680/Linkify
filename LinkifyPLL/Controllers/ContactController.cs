using LinkifyBLL.ModelView;
using LinkifyBLL.Services.Abstraction;
using LinkifyDAL.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace LinkifyPLL.Controllers
{
    public class ContactController : Controller
    {
        private readonly IContactService _contactService;
        private readonly UserManager<User> _userManager;
        public ContactController(IContactService contactService, UserManager<User> userManager)
        {
            _contactService = contactService;
            _userManager = userManager;
        }
        public async Task <IActionResult> ContactInfo()
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

        [HttpGet]
        public IActionResult AddContact()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> AddContact(AddContactVM model)
        {
            if (!ModelState.IsValid)
                return View(model);
            //validate based on type
            if(model.Type == "Email" && !new EmailAddressAttribute().IsValid(model.Value))
            {
                ModelState.AddModelError("Value", "Invalid email format");
                return View(model);
            }
            if(model.Type == "Phone" && !Regex.IsMatch(model.Value, @"^\+?\d{8,15}$"))
            {
                ModelState.AddModelError("Value", "Invalid phone number format");
                return View(model);
            }
            var user = await _userManager.GetUserAsync(User);
            _contactService.AddContact(model, user.Id);
            return RedirectToAction("ContactInfo");
        }

    }
}
