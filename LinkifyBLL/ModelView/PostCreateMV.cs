using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;


namespace LinkifyBLL.ModelView
{
    public class PostCreateMV
    {
        [Required]
        public string TextContent { get; set; }
        public List<IFormFile>? Images { get; set; }
        public IFormFile ImageFile { get; set; }
        public IFormFile VideoFile { get; set; }
        public IFormFile DocumentFile { get; set; }
        public string Visibility { get; set; } = "public";

    }
}



   
