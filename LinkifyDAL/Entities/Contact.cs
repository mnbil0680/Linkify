using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LinkifyDAL.Entities
{
    public class Contact
    {
        public int Id { get; private set; }
        [Required]
        public string Type { get; private set; } // e.g., "Email", "Phone", "Social Media"
        [Required]
        public string Value { get; private set; }
        [Required]
        public string UserId { get; set; } // FK to Identity User
        public DateTime CreatedOn { get; set; } = DateTime.UtcNow;
        public DateTime? ModifiedOn { get; set; }
        public DateTime? DeletedOn { get; set; }
    }
}
