using System;

namespace LinkifyDAL.Entities
{
    public class PostImages
    {
        public int Id { get; private set; }
        public string ImagePath { get; private set; }
        public int PostId { get; private set; }
        public bool IsDeleted { get; private set; } = false;
        public DateTime CreatedOn { get; private set; } = DateTime.UtcNow;
        public DateTime? UpdatedOn { get; private set; }
        public DateTime? DeletedOn { get; private set; }
        public PostImages(string ImagePath, int PostId) { 
            this.ImagePath = ImagePath;
            this.PostId = PostId;
        }
        public void Update(string imagePath)
        {
            this.ImagePath = imagePath;
            UpdatedOn = DateTime.UtcNow;
        }
        public void Delete()
        {
            IsDeleted = true;
            DeletedOn = DateTime.UtcNow;
        }
    }
}
