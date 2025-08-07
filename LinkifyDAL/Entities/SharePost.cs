using System.ComponentModel.DataAnnotations.Schema;

namespace LinkifyDAL.Entities
{
    public class SharePost
    {
        public int Id { get; private set; }
        public int PostId { get; private set; }
        public string UserId { get; private set; }
        public DateTime SharedAt { get; private set; } = DateTime.Now;
        public string? Caption { get; private set; }
        public bool IsArchived { get; private set; } = false;
        public DateTime? ArchivedAt { get; private set; }

        [ForeignKey(nameof(PostId))]
        public Post Post { get; private set; }

        [ForeignKey(nameof(UserId))]
        public User User { get; private set; }

        public SharePost(int postId, string userId, string? caption = null)
        {
            PostId = postId;
            UserId = userId;
            Caption = caption;
        }
        public void UpdateCaption(string newCaption)
        {
            Caption = newCaption;
        }

        public void Archive()
        {
            IsArchived = true;
            ArchivedAt = DateTime.Now;
        }

        public void Restore()
        {
            IsArchived = false;
            ArchivedAt = null;
        }
    }
}
