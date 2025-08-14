using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LinkifyBLL.ModelView
{
    public class SavedJobsMV
    {
        public List<JobListItemMV> Jobs { get; set; }
        public int SavedJobsCount { get; set; }
        public int AppliedJobsCount { get; set; }
        public string CurrentFilter { get; set; }
    }
}
