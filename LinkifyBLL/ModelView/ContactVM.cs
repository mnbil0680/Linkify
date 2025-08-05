using System.ComponentModel.DataAnnotations;

namespace LinkifyBLL.ModelView
{
    public class ContactVM // ViewModel for displaying contact information
    {
        public string MainEmail { get; set; } // from IdentityUser.Email
        public List<ContactDTO>? Contacts { get; set; } // for other emails, phone numbers.
    }
}
