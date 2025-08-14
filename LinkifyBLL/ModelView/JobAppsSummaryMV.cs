using LinkifyDAL.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LinkifyBLL.ModelView
{
    public class JobAppsSummaryMV
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public ApplicationStatus Status { get; set; } = 0; // Pending, Interviewing, Accepted, Rejected
        public DateTime AppliedOn { get; set; }
        public string CoverLetter { get; set; } // Optional cover letter text
        //public string? Experience { get; set; } // Years of experience
        public String ResumeUrl { get; set; }

  
    }
}
