using LinkifyDAL.Enums;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LinkifyBLL.ModelView
{
    public class ApplicationDetailsMV
    {
        // Application info
        public int Id { get; set; }
        public int JobId { get; set; } // ID of the job this application is for
        public DateTime AppliedOn { get; set; } // When the application was submitted
        public string Status { get; set; } = "pending"; // Pending, Interviewing, Accepted, Rejected

        // Applicant info
        [Required(ErrorMessage = "First Name is required.")]
        public string FirstName { get; set; }
        [Required(ErrorMessage = "Last Name is required.")]
        public string LastName { get; set; }
        [Required, EmailAddress(ErrorMessage = "Invalid email address.")]
        public string Email { get; set; }
        [Phone]
        public string Phone { get; set; }
        public string Location { get; set; } // Location of the applicant
        
        public string LinkedInProfile { get; set; } // URL to LinkedIn profile

        public IFormFile CoverLetter { get; set; } // Optional cover letter text
        public string CoverLetterPath { get; set; } = null;
        [Required]
        public string Experience { get; set; }
        [MaxLength(500, ErrorMessage = "Motivation cannot exceed 500 characters.")]
        public string Motivation { get; set; } // Why the applicant is interested in this job

        // Job info
        public DateTime JobCreatedOn { get; set; }

        [Range(typeof(bool), "true", "true", ErrorMessage = "You must agree to the terms and conditions.")]
        public bool TermsAccepted { get; set; } // Checkbox for terms and conditions
        public bool RecieveUpdates { get; set; } // Checkbox for receiving updates
    }
}
