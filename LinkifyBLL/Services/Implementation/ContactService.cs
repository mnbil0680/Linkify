using LinkifyBLL.ModelView;
using LinkifyBLL.Services.Abstraction;
using LinkifyDAL.Entities;
using LinkifyDAL.Repo.Abstraction;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace LinkifyBLL.Services.Implementation
{
    public class ContactService : IContactService
    {
        private readonly IContactRepository _contactRepository;

        private readonly UserManager<User> _userManager;
            public ContactService(IContactRepository contactRepository, UserManager<User> userManager)

        {
            _contactRepository = contactRepository;
            _userManager = userManager;
        }
        public List<ContactDTO> GetContactsByUserId(string userId)
        {
            var contacts = _contactRepository.GetByUserId(userId);
            var contactDTOs = contacts.Select(c => new ContactDTO
            {
                Id = c.Id,
                Type = c.Type,
                Value = c.Value,
            }).ToList();
            return contactDTOs;
        }
        public void DeleteContact(int id)
        {
            _contactRepository.Delete(id);
        }

        public bool AddContact(AddContactVM model, string userId, out string errorMessage)
        {
            errorMessage = string.Empty;

            if (string.IsNullOrWhiteSpace(model.Value))
            {
                errorMessage = "Value is required.";
                return false;
            }

            if (model.Type == "Email" && !new EmailAddressAttribute().IsValid(model.Value))
            {
                errorMessage = "Invalid email format.";
                return false;
            }

            if (model.Type == "Phone" && !Regex.IsMatch(model.Value, @"^\+?\d{8,15}$"))
            {
                errorMessage = "Invalid phone number format.";
                return false;
            }

            if (_contactRepository.Exists(model.Type, model.Value, userId))
            {
                errorMessage = "This contact already exists.";
                return false;
            }

            var contact = new Contact(model.Type, model.Value, userId);
            _contactRepository.Add(contact);
            return true;
        }


    }
}
