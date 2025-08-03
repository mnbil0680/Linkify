using LinkifyBLL.ModelView;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LinkifyBLL.Services.Abstraction
{
    public interface IContactService
    {
        public List<ContactDTO> GetContactsByUserId(string userId);
        public void DeleteContact(int id);
        public void AddContact(AddContactVM model, string userId);
    }
}
