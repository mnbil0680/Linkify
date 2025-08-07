using LinkifyDAL.Entities;
namespace LinkifyDAL.Repo.Abstraction
{
    public interface IContactRepository
    {
        public List<Contact> GetByUserId(string userId);
        public Contact GetContactId(int id);
        public void Delete(int id);
        public void Add(Contact contact);
        public bool Exists(string type, string value, string userId);
    }
}
