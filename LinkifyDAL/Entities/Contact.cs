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

        public string UserId { get; private set; } // FK to Identity User
        public bool? IsDeleted { get; private set; } = false;
        public DateTime CreatedOn { get; private set; } = DateTime.UtcNow;
        public DateTime? ModifiedOn { get; private set; }
        public DateTime? DeletedOn { get; private set; }
        
        public Contact(string type, string value, string userId)
        {
            Type = type;
            Value = value;
            UserId = userId;
            CreatedOn = DateTime.UtcNow;
            IsDeleted = false;
        }

        public bool MarkedAsDeleted()
        {
            if (IsDeleted == true)
            {
                return false; // Already deleted
            }
            IsDeleted = true;
            DeletedOn = DateTime.UtcNow;
            return true; // Successfully marked as deleted
        }
    }
}
