using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LinkifyBLL.ModelView
{
    public class PostMV
    {
        public int Id { get; set; }
        public string TextContent { get; set; }
        public string UserName { get; set; }
        public string UserAvatar { get; set; }
        public DateTime CreatedAt { get; set; }
        public List<string> Images { get; set; }

        //no of reactions 
        public int ReactionCount { get; set; }
        public int LikeCount { get; set; }
        public int LoveCount { get; set; }
        public int LaughCount { get; set; }
        public int SadCount { get; set; }
        public int AngryCount { get; set; }

    }
}
