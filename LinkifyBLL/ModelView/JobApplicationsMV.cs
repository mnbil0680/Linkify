using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LinkifyBLL.ModelView
{
    public class JobApplicationsMV
    {
        public int JobId { get; set; }
        public string JobTitle { get; set; }
        public string JobCompany { get; set; }
        public string JobLocation { get; set; }
        public string JobType { get; set; } // FullTime, PartTime, Contract, Internship
        public DateTime? ExpiresOn { get; set; }
        public DateTime JobCreatedOn { get; set; }

        // Statistics
        public int TotalApplications { get; set; }
        public int PendingApplications { get; set; }
        public int InterviewingApplications { get; set; }
        public int AcceptedApplications { get; set; }

        // Filter
        public string CurrentFilter { get; set; }
        public List<JobAppsSummaryMV>? Applications { get; set; } 
    }
}
