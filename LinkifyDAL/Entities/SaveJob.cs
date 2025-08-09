using System.ComponentModel.DataAnnotations.Schema;

namespace LinkifyDAL.Entities
{
    public class SaveJob
    {
        public int Id { get; private set; }
        public int JobId { get; private set; }
        public string UserId { get; private set; }
        public DateTime SavedOn { get; private set; } = DateTime.Now;
        public bool IsArchived { get; private set; } = false;
        public DateTime? DeletedOn { get; private set; }

        [ForeignKey(nameof(JobId))]
        public Job Job { get; private set; }

        [ForeignKey(nameof(UserId))]
        public User User { get; private set; }

        public SaveJob(int jobId, string userId)
        {
            JobId = jobId;
            UserId = userId;
        }

        public void Delete()
        {
            IsArchived = true;
            DeletedOn = DateTime.Now;
        }

        public void Restore()
        {
            IsArchived = false;
            DeletedOn = null;
        }
    }
}
