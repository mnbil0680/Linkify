using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LinkifyBLL.ModelView
{
    public class PostReactionMV
    {
        public int Id { get; set; }                // Reaction ID
        public int PostId { get; set; }            // The post this reaction belongs to
        public string ReactorId { get; set; }      // The user who reacted
        public string ReactorUserName { get; set; } // (Optional) For display
        public string Reaction { get; set; }       // e.g., "Like", "Love", etc.
        public bool IsDeleted { get; set; }        // Soft delete flag
        public DateTime CreatedOn { get; set; }    // When the reaction was made
    }
}

