using System.ComponentModel.DataAnnotations.Schema;

namespace LinkifyDAL.Entities
{
    public class SavePost
    {
        public int Id { get; private set; }
        public int PostId { get; private set; }
        public string UserId { get; private set; }
        public DateTime SavedAt { get; private set; } = DateTime.Now;
        
        //IsArchived == IsDeleted/ we changed the name for clarity
        public bool IsArchived { get; private set; } = false;
        public DateTime? ArchivedAt { get; private set; }

        [ForeignKey(nameof(PostId))]
        public Post Post { get; private set; }

        [ForeignKey(nameof(UserId))]
        public User User { get; private set; }
        public SavePost(int postId, string userId)
        {
            this.PostId = postId;
            this.UserId = userId;
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
