using System.ComponentModel.DataAnnotations.Schema;

namespace LinkifyDAL.Entities
{
    public class PostComments
    {
        public int Id { get; private set; }
        public string Content { get; private set; }
        public int PostId { get; private set; }
        public string CommenterId { get; private set; }
        public string? ImgPath { get; private set; }
        public bool IsDeleted { get; private set; } = false;
        public DateTime CreatedOn { get; private set; } = DateTime.Now;
        public DateTime? UpdatedOn { get; private set; }
        public DateTime? DeletedOn { get; private set; }

        [ForeignKey(nameof(PostId))]
        public Post Post { get; private set; }

        [ForeignKey(nameof(CommenterId))]
        public User User { get; private set; }
        public int? ParentCommentId { get; private set; }
        [ForeignKey(nameof(ParentCommentId))]
        public PostComments ParentComment { get; private set; }
        public PostComments(string content, int postId, string commenterId, string imgPath, int? parentCommentId = null)
        {
            this.Content = content;
            this.PostId = postId;
            this.CommenterId = commenterId;
            this.ImgPath = imgPath;
            this.ParentCommentId = parentCommentId;
        }
        public void Update(string newContent, string? imgPath = null)
        {
            Content = newContent;
            if(imgPath != null) this.ImgPath = imgPath;
            UpdatedOn = DateTime.Now;
        }
        public void Delete()
        {
            IsDeleted = true;
            DeletedOn = DateTime.UtcNow;
        }
    }
}
