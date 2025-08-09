using System.ComponentModel.DataAnnotations.Schema;

namespace LinkifyDAL.Entities
{
    public class Post
    {
        public int Id { get; private set; }
        public string? TextContent { get; private set; }
        public bool IsDeleted { get; private set; } = false;
        public DateTime CreatedOn { get; private set; } = DateTime.Now;
        public DateTime? UpdatedOn { get; private set; }
        public DateTime? DeletedOn { get; private set; }
        public string UserId { get; private set; }

        [ForeignKey(nameof(UserId))]
        public User User { get; private set; }
        
        public void Edit(string textContent)
        {
            this.TextContent = textContent;
            UpdatedOn = DateTime.Now;
        }
        public void Delete()
        {
            IsDeleted = true;
            this.DeletedOn = DateTime.Now;
        }
    }
}
