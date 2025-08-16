using LinkifyDAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace LinkifyBLL.ModelView
{
    public class PostMV
    {
        public int postId { get; set; }
        public string PostUserName { get; set; }
        public string PostUserId { get; set; }
        public string PostUserTitle { get; set; }
        public string PostUserImg { get; set; }
        public bool IsSavedByCurrentUser { get; set; }
        public bool IsEdited { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool IsPremiumUser { get; set; } = true;
        public bool IsVerified { get; set; } = true;

        public bool IsSharedPost { get; set; } = false;
        public User SharedPostAuthorId { get; set; }
        public User SharedById { get; set; }
        public string? SharedCaption { get; set; }
        public DateTime SharedAt { get; set; }

        public string TextContent { get; set; }
        public TimeSpan Since { get; set; }
        public List<string> Images { get; set; }
        public int imageCount { get; set; }



        public List<CommentCreateMV> Comments { get; set; }
        public int CommentsCount { get; set; }
        public int NumberOfShares { get; set; }

        //no of reactions 
        // index   :  0     1      2     3    4   
        // Reaction: Like  Love   Haha  Sad  Angry 
        public List<int> ReactionsNumbers { get; set; }
        public List<PostReactionMV> Reactions { get; set; }
        public int ReactionCount { get; set; }
        public int LikeCount { get; set; }
        public int LoveCount { get; set; }
        public int LaughCount { get; set; }
        public int SadCount { get; set; }
        public int AngryCount { get; set; }

    }
}
