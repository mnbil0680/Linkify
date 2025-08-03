
using System.ComponentModel.DataAnnotations;

namespace LinkifyBLL.ModelView
{
    public class AddContactVM
    {
        [Required(ErrorMessage = "Type is required.")]
        public string Type { get; set; } // e.g., "Email", "Phone", "Social Media"
        [Required(ErrorMessage = "Value is required.")]
        public string Value { get; set; }
    }
}
