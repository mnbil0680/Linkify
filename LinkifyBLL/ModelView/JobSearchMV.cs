using LinkifyDAL.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LinkifyBLL.ModelView
{
    public class JobSearchMV
    {
        public string? Keyword { get; set; }
        public string? Location { get; set; }
        public string? Company { get; set; }
        public JobTypes? Type { get; set; } // FullTime, PartTime, Contract, Internship
        public JobPresence? Presence { get; set; } // Remote, Onsite, Hybrid
        public string? Sort { get; set; }
        public IEnumerable<JobMV>? Jobs { get; set; }
        public bool Applied { get; set; }
        public bool IsSaved { get; set; }
 
    }
}
