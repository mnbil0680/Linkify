using LinkifyDAL.Entities;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LinkifyBLL.Services.Abstraction
{
    public interface IPostService
    {
        Task<Post> CreatePostAsync(string userId, string textContent, List<IFormFile> images);
    }
}
