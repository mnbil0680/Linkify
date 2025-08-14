using LinkifyDAL.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LinkifyBLL.ModelView
{
    public class AddJobMV
    {
        
        [Required(ErrorMessage="Title is required")]
        public string Title { get; set; }
        [Required(ErrorMessage="Description is required")]
        public string Description { get; set; }
        [Required(ErrorMessage="Company name is required")]
        public string Company { get; set; }
        [Required(ErrorMessage="Location is required")]
        public string Location { get; set; }
        [Required(ErrorMessage="Salary range is required")]
        public string SalaryRange { get; set; }
        public JobTypes Type { get; set; } = JobTypes.FullTime;
        public JobPresence Presence { get; set; } = JobPresence.Onsite;
        public DateTime? ExpiresOn { get; set; }
       
    }
}
