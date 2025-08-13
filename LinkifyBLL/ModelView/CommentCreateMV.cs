using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.Design;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LinkifyBLL.ModelView
{
    public class CommentCreateMV
    {
        [Required]
        public int PostId { get; set; }
        public int CommentID { get; set; }
        public string TextContent { get; set; }
        public string? ImagePath { get; set; }
        public int? ParentCommentId { get; set; }  // null = it's a top-level comment
        public string? CommenterId { get; set; }

        public string AuthorName { get; set; }
        public string AuthorAvatar { get; set; }

        public List<CommentReactionMV> Reactions { get; set; }
        public List<CommentCreateMV> Replies { get; set; }
        public TimeSpan Since { get; set; }
        public bool IsEdited { get; set; }
        public DateTime CreatedAt { get; set; }

        public CommentCreateMV(bool isEdited,DateTime createdAt,string authorName, string authorAvatar, int commentId, int postId, string textContent, string? imagePath = null, int? parentCommentId = null, string? commenterId =null)
        {
            CreatedAt = createdAt;
            IsEdited = isEdited;
            AuthorAvatar = authorAvatar;
            AuthorName = authorName;
            CommentID = commentId;
            PostId = postId;
            TextContent = textContent;
            ImagePath = imagePath;
            ParentCommentId = parentCommentId;
            CommenterId = commenterId;
        }

        public CommentCreateMV( int commentId, int postId, string textContent, string? imagePath = null, int? parentCommentId = null, string? commenterId = null)
        {

            CommentID = commentId;
            PostId = postId;
            TextContent = textContent;
            ImagePath = imagePath;
            ParentCommentId = parentCommentId;
            CommenterId = commenterId;
        }
    }
}
