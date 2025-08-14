using LinkifyDAL.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LinkifyBLL.ModelView
{
    public class JobMV
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Company { get; set; }
        public string Location { get; set; }
        public JobTypes Type { get; set; }
        public JobPresence Presence { get; set; }
        public string Description { get; set; }
        public string SalaryRange { get; set; }
        public DateTime CreatedOn { get; set; }   
        public bool IsSaved { get; set; } = false;
        public bool Applied { get; set; } = false;
    }
}
