using LinkifyBLL.ModelView;

namespace LinkifyBLL.Services.Abstraction
{
    public interface IContactService
    {
        public List<ContactDTO> GetContactsByUserId(string userId);
        public void DeleteContact(int id);
        public bool AddContact(AddContactVM model, string userId, out string errorMessage);
    }
}
