using LinkifyDAL.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LinkifyBLL.ModelView
{
    public class RightSideJob
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Company { get; set; }
        public JobPresence Presence { get; set; }
        public string Location { get; set; }
        public string Salary { get; set; }

    }
}
