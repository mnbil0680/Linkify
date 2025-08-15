using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LinkifyBLL.ModelView
{
    public class RightSide
    {
        public List<RightSideConnection> Connections { get; set; }
        public List<RightSideJob> Jobs { get; set; }
    }
}
