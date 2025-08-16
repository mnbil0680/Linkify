using LinkifyDAL.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace LinkifyDAL.Entities
{
    public class Certificate
    {
        public int Id { get; private set; }
        public string Name { get; private set; }
        public string CertificatePath { get; private set; }
        public string IssuingOrganization { get; private set; }
        public DateTime? IssueDate { get; private set; }
        public DateTime? ExpirationDate { get; private set; }
        public string? CredentialId { get; private set; }
        public string? CredentialUrl { get; private set; }
        public CertificateStatus Status { get; private set; } = CertificateStatus.Active;
        public bool IsDeleted { get; private set; } = false;
        public DateTime CreatedOn { get; private set; } = DateTime.Now;
        public DateTime? UpdatedOn { get; private set; }
        public DateTime? DeletedOn { get; private set; }
        public string UserId { get; private set; }

        [ForeignKey(nameof(UserId))]
        public User User { get; private set; }

        public Certificate(
            string userId,
            string certificatePath,
            string name,
            string issuingOrganization,
            DateTime? issueDate,
            DateTime? expirationDate,
            string? credentialId,
            string? credentialUrl)
        {
            UserId = userId;
            CertificatePath = certificatePath;
            Name = name;
            IssuingOrganization = issuingOrganization;
            IssueDate = issueDate;
            ExpirationDate = expirationDate;
            CredentialId = credentialId;
            CredentialUrl = credentialUrl;
        }

        public void Edit(
            string? name,
            string? certificatePath,
            string? issuingOrganization,
            DateTime? issueDate,
            DateTime? expirationDate,
            string? credentialId,
            string? credentialUrl,
            CertificateStatus? status)
        {
            this.UpdatedOn = DateTime.Now;

            if (name != null) this.Name = name;
            if (certificatePath != null) this.CertificatePath = certificatePath;
            if (issuingOrganization != null) this.IssuingOrganization = issuingOrganization;
            if (issueDate.HasValue) this.IssueDate = issueDate.Value;
            if (expirationDate.HasValue) this.ExpirationDate = expirationDate;
            if (credentialId != null) this.CredentialId = credentialId;
            if (credentialUrl != null) this.CredentialUrl = credentialUrl;
            if (status != null) this.Status = (CertificateStatus)status;
        }

        public void Delete()
        {
            IsDeleted = true;
            DeletedOn = DateTime.Now;
        }

        public void Restore()
        {
            this.UpdatedOn = DateTime.Now;
            IsDeleted = false;
            DeletedOn = null;
        }

        public void UpdateStatus(CertificateStatus newStatus)
        {
            Status = newStatus;
            UpdatedOn = DateTime.Now;
        }
    }
}
