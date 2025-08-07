using LinkifyDAL.Enums;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations.Schema;

namespace LinkifyDAL.Entities
{
    public class Job
    {
        public int Id { get; private set; }
        public string Title { get; private set; }
        public string Description { get; private set; }
        public string Company { get; private set; }
        public string? Location { get; private set; }
        public string? SalaryRange { get; private set; }
        public bool? IsActive { get; private set; } = true;
        public JobTypes? Type { get; private set; } = JobTypes.FullTime;
        public JobPresence? Presence { get; private set; } = JobPresence.Onsite;
        public DateTime CreatedOn { get; private set; } = DateTime.Now;
        public DateTime? ExpiresOn { get; private set; }
        public bool? IsDeleted { get; private set; } = false;
        public DateTime? DeletedOn { get; private set; }
        public DateTime? UpdatedOn { get; private set; }
        public string UserId { get; private set; }

        [ForeignKey(nameof(UserId))]
        public User User { get; private set; }

        public Job(string userId, string title, string description, string company, string location, string salaryRange, JobTypes? type, JobPresence? presence, DateTime? expiresOn) { 
            this.UserId = userId;
            this.Title = title;
            this.Description = description;
            this.Company = company;
            this.Location = location;
            this.SalaryRange = salaryRange;
            if (type != null) this.Type = (JobTypes)type;
            if (presence != null) this.Presence = (JobPresence)presence;
            if (expiresOn.HasValue) this.ExpiresOn = expiresOn;
        }
        public void Edit(string? title, string? description, string? company, string? location, string? salaryRange, JobTypes? type, JobPresence? presence, DateTime? expiresOn) {
            this.UpdatedOn = DateTime.Now;
            if (title != null) this.Title = title;
            if (description != null) this.Description = description;
            if (company != null) this.Company = company;
            if (location != null) this.Location = location;
            if (salaryRange != null) this.SalaryRange = salaryRange;
            if (type != null) this.Type = (JobTypes)type;
            if (presence != null) this.Presence = (JobPresence)presence;
            if (expiresOn.HasValue) this.ExpiresOn = expiresOn;
        }
        public void Delete() { 
            IsDeleted = true;
            DeletedOn = DateTime.Now;
        }
        public void Restore() {
            this.UpdatedOn = DateTime.Now;
            IsDeleted = false;
            DeletedOn = null;
        }

        public void ToggleActivation() { 
            if ((bool)IsActive) IsActive = false;
            else IsActive = true;
        }
    }
}
