using LinkifyDAL.Enums;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace LinkifyDAL.Entities
{
    public class User : IdentityUser
    {
        public string? ImgPath { get; private set; }
        public string? CVPath { get; private set; }
        public UserStatus? Status { get; private set; } = UserStatus.opentowork;
        public string? Title { get; private set; }
        public string? Bio { get; private set; }
        public bool IsDeleted { get; private set; } = false;
        public DateTime? DeletedOn { get; private set; }
        public DateTime? RegistrationDate { get; private set; } = DateTime.Now;
        public DateTime? UpdatedOn { get; private set; }

        public User(string? userName, string? email, string? imgPath, string? CVPath, UserStatus? status, string? title, string? bio)
        {
            this.UserName = userName;
            this.Email = email;
            this.ImgPath = imgPath;
            this.CVPath = CVPath;
            this.Status = status;
            this.Title = title;
            this.Bio = bio;
        }
        public void Edit(string? userName, string? imgPath, string? CVPath, string? title, string? bio)
        {
            if (userName != null)
                this.UserName = userName;
            if (imgPath != null)
                this.ImgPath = imgPath;
            if (CVPath != null)
                this.CVPath = CVPath;
            this.UpdatedOn = DateTime.Now;
            this.CVPath = CVPath ?? string.Empty;
        }
        public void UpdateStatus(UserStatus newStatus)
        {
            Status = newStatus;
            UpdatedOn = DateTime.Now;
        }
        public void Delete()
        {
            this.IsDeleted = true;
            DeletedOn = DateTime.Now;
        }
    }
}
