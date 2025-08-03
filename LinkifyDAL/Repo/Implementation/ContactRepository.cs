using LinkifyDAL.DataBase;
using LinkifyDAL.Entities;
using LinkifyDAL.Repo.Abstraction;


namespace LinkifyDAL.Repo.Implementation
{
    public class ContactRepository : IContactRepository
    {
        private readonly LinkifyDbContext _context;
        public ContactRepository(LinkifyDbContext context)
        {
            _context = context;
        }
        public List<Contact> GetByUserId(string userId)
        {
            if (string.IsNullOrEmpty(userId))
            {
                throw new ArgumentException("User ID cannot be null or empty", nameof(userId));
            }
            return _context.Contacts
                 .Where(c => c.UserId == userId && c.DeletedOn == null)
                 .ToList(); // fetches data from DB first
        }
        public Contact GetContactId(int id)
        {
            if (id <= 0)
            {
                throw new ArgumentException("Invalid contact ID", nameof(id));
            }
            return _context.Contacts
                .FirstOrDefault(c => c.Id == id);
        }
        public void Delete(int id)
        {
            if (id <= 0)
            {
                throw new ArgumentException("Invalid contact ID", nameof(id));
            }
            var contact = GetContactId(id);
            if (contact == null)
            {
                throw new KeyNotFoundException($"Contact with ID {id} not found.");
            }
            contact.MarkedAsDeleted();
            _context.SaveChanges();
        }
        public void Add(Contact contact)
        {
            _context.Contacts.Add(contact);
            _context.SaveChanges();
        }
    }
}
