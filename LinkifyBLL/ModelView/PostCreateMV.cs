using LinkifyDAL.Entities;
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;


namespace LinkifyBLL.ModelView
{
    public class PostCreateMV
    {
        
        public string? TextContent { get; set; }
        public List<IFormFile>? Images { get; set; }
       

    }
}



   
