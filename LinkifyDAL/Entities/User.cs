using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace LinkifyDAL.Entities
{
    public class User : IdentityUser
    {
        public string? ImgPath { get; private set; }
        public string? Status { get; private set; }
        public string? Title { get; private set; }
        public string? Bio { get; private set; }
        public bool IsDeleted { get; private set; } = false;
        public DateTime? DeletedOn { get; private set; }
        public DateTime? RegistrationDate { get; private set; } = DateTime.Now;
        public DateTime? UpdatedOn { get; private set; }

        public User(string? userName, string? email, string? imgPath, string? status, string? title, string? bio)
        {
            this.UserName = userName;
            this.Email = email;
            this.ImgPath = imgPath;
            this.Status = status;
            this.Title = title;
            this.Bio = bio;
        }
        public void Edit(string? userName, string? imgPath, string? status)
        {
            if (userName != null)
                this.UserName = userName;
            this.ImgPath = imgPath;
            this.Status = status;
            this.UpdatedOn = DateTime.Now;
        }
        public void Delete()
        {
            this.IsDeleted = true;
            DeletedOn = DateTime.Now;
        }

        public void AddTitle(string title) { 
            this.Title = title;
        }
        public void AddBio(string bio)
        {
            this.Bio = bio;
        }
    }
}
