using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LinkifyBLL.ModelView
{
    public class PostMV
    {
        public string PostUserName { get; set; }
        public string PostUserTitle { get; set; }
        public string PostUserImg { get; set; }



        public string TextContent { get; set; }
        public TimeSpan Since { get; set; }
        public List<string> Images { get; set; }
        public int imageCount { get; set; }



        //public List<CommentCreateMV> Comments { get; set; }
        public int CommentsCount { get; set; }

        public int NumberOfShares { get; set; }

        //no of reactions 
        public int ReactionCount { get; set; }
        public int LikeCount { get; set; }
        public int LoveCount { get; set; }
        public int LaughCount { get; set; }
        public int SadCount { get; set; }
        public int AngryCount { get; set; }

    }
}
