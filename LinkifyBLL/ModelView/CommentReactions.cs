using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LinkifyBLL.ModelView
{
    public class CommentReactionMV
    {
        public int Id { get; set; } // Reaction ID (optional, for updates/deletes)
        public int CommentId { get; set; }
        public string ReactorId { get; set; }
        public string ReactorUserName { get; set; } // Optional: for display
        public string Reaction { get; set; } // e.g., "Like", "Love", etc.
        public bool IsDeleted { get; set; }
        public DateTime CreatedOn { get; set; }
    }
}

