// File: LinkifyBLL/ModelView/EditPostMV.cs
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace LinkifyBLL.ModelView
{
    public class EditPostMV
    {
        public int PostId { get; set; }

        public string? NewTextContent { get; set; }

        // For new image uploads
        public List<IFormFile>? NewImgPaths { get; set; }

        // For existing images
        public List<string>? ExistingImages { get; set; }
    }
}
