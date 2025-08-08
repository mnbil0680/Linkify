using LinkifyDAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LinkifyBLL.ModelView
{
    public class HomeMV
    {

        //For Identity User

        public List<PostImages> PostImages{get; set;}
        public List<SharePost> SharePosts { get; set; }
        public List<PostReactions> PostReactions { get; set; }
        public List<PostComments> PostComments { get; set; }
        public List<Post> Posts { get; set; }

    }
}
