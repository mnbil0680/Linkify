using LinkifyDAL.Enums;
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace LinkifyBLL.ModelView
{
    public class CertificateMV
    {
        public int Id { get; set; }
        public string? UserId { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; }
        public IFormFile? Certificate { get; set; }

        public string? CertificatePath { get; set; }

        [Required]
        [StringLength(100)]
        public string? IssuingOrganization { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [Display(Name = "Issue Date")]
        public DateTime? IssueDate { get; set; } = DateTime.Today;

        [DataType(DataType.Date)]
        [Display(Name = "Expiration Date")]
        public DateTime? ExpirationDate { get; set; }

        [Display(Name = "Credential ID")]
        [StringLength(50)]
        public string? CredentialId { get; set; }

        [Url]
        [Display(Name = "Credential URL")]
        public string? CredentialUrl { get; set; }

        public CertificateStatus Status { get; set; } = CertificateStatus.Active;
    }
}
