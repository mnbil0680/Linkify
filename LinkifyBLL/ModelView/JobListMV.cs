using LinkifyDAL.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LinkifyBLL.ModelView
{
    public class JobListMV
    {
        public List<JobListItemMV>? jobs { get; set; }
        public int? ApplicationsCount { get; set; }
        public int? ActiveJobsCount { get; set; }
        public int? TotalJobsCount { get; set; }
        public string? CurrentFilter { get; set; } = "all";
    }
}
