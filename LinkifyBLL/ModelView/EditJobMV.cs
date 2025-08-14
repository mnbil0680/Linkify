using LinkifyDAL.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LinkifyBLL.ModelView
{
    public class EditJobMV
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Company { get; set; }
        public string Location { get; set; }
        public string Description { get; set; }
        public DateTime? ExpiresOn { get; set; }
        public JobTypes Type { get; set; } // FullTime, PartTime, Contract, Internship
        public JobPresence Presence { get; set; } // Onsite, Hybrid, Remote
        public string SalaryRange { get; set; }
        public bool IsActive { get; set; } = true;

    }
}
