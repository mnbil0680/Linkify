using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LinkifyBLL.ModelView
{
    public class CommentCreateMV
    {
        [Required]
        public int PostId { get; set; }
        public string TextContent { get; set; }
        public string? ImagePath { get; set; }
        public int? ParentCommentId { get; set; }  // null = it's a top-level comment

        public CommentCreateMV(int postId, string textContent, string? imagePath = null, int? parentCommentId = null)
        {
            PostId = postId;
            TextContent = textContent;
            ImagePath = imagePath;
            ParentCommentId = parentCommentId;
        }
    }
}
