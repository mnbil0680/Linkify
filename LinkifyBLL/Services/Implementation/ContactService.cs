using LinkifyBLL.ModelView;
using LinkifyBLL.Services.Abstraction;
using LinkifyDAL.Entities;
using LinkifyDAL.Repo.Abstraction;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        public void AddContact(AddContactVM model, string userId)
        {
            if (model == null)
            {
                throw new ArgumentNullException(nameof(model), "Model cannot be null");
            }
            if (string.IsNullOrEmpty(userId))
            {
                throw new ArgumentException("User ID cannot be null or empty", nameof(userId));
            }
            var contact = new Contact(model.Type, model.Value, userId);
            
            _contactRepository.Add(contact);
        }

    }
}
