using LinkifyDAL.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LinkifyBLL.ModelView
{
    public class AddReactionMV
    {
        public int PostId { get; set; }
        public ReactionTypes Reaction { get; set; } // e.g., "Like", "Love", "Haha", etc.
    }
}
