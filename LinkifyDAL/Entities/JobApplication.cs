using LinkifyDAL.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace LinkifyDAL.Entities
{
    public class JobApplication
    {
        public int Id { get; private set; }
        public int JobId { get; private set; }
        public string ApplicantId { get; private set; }
        public DateTime AppliedOn { get; private set; } = DateTime.Now;
        public ApplicationStatus Status { get; private set; } = ApplicationStatus.Pending;
        public string? CoverLetter { get; private set; }
        public DateTime? UpdatedOn { get; private set; }
        public bool? IsDeleted { get; private set; } = false;
        public DateTime? DeletedOn { get; private set; }

        [ForeignKey(nameof(JobId))]
        public Job Job { get; private set; }

        [ForeignKey(nameof(ApplicantId))]
        public User Applicant { get; private set; }

        public JobApplication(int jobId, string applicantId, string? coverLetter = null)
        {
            JobId = jobId;
            ApplicantId = applicantId;
            CoverLetter = coverLetter;
        }
        public void UpdateStatus(ApplicationStatus newStatus)
        {
            if (Status == ApplicationStatus.Archived)
                throw new InvalidOperationException("Cannot modify archived applications");

            Status = newStatus;
            UpdatedOn = DateTime.Now;
        }
        public void UpdateCoverLetter(string newCoverLetter)
        {
            if (IsDeleted ?? false)
                throw new InvalidOperationException("Cannot modify archived applications");
            CoverLetter = newCoverLetter;
            UpdatedOn = DateTime.Now;
        }
        public void Delete()
        {
            IsDeleted = true;
            DeletedOn = DateTime.Now;
            Status = ApplicationStatus.Archived;
        }
        public void Restore()
        {
            IsDeleted = false;
            DeletedOn = null;
            UpdatedOn = DateTime.Now;
        }
    }
}
