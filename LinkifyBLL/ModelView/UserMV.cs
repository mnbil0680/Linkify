using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace LinkifyBLL.ModelView
{
    public class UserMV
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string? Email { get; set; }
        public string? Password { get; set; }
        public string? ImgPath { get; set; }
        public string? Status { get; set; }
        public IFormFile? Image { get; set; } = null;
        public int? ConnectionsCount { get; set; } = 0;
        public int? PostsCount { get; set; } = 0;
        public int? ViewsCount { get; set; } = 0;
        public int? InvitationCount { get; set; } = 0;
        public int? SavedJobsCount { get; set; } = 0;
        public int? CertificationCount { get; set; } = 0;

        public UserMV(string id, string name, string email, string password, string imgPath, string status) { 
            this.Id = id;
            this.Name = name;
            this.Email = email;
            this.Password = password;
            this.ImgPath = imgPath;
            this.Status = status;

        }
    }
}
