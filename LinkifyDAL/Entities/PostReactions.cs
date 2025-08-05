using LinkifyDAL.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace LinkifyDAL.Entities
{
    public class PostReactions
    {
        public int Id { get; private set; }
        public ReactionTypes Reaction {  get; private set; }
        public string ReactorId { get; private set; }

        [ForeignKey(nameof(ReactorId))]
        public User Reactor { get; private set; }
        public int PostId { get; private set; }

        [ForeignKey(nameof(PostId))]
        public Post Post { get; private set; }
        public bool IsDeleted { get; private set; } = false;
        public DateTime CreatedOn { get; private set; } = DateTime.Now;
        public DateTime? UpdatedOn { get; private set; }
        public DateTime? DeletedOn { get; private set; }
        public PostReactions(ReactionTypes reaction, string reactorId, int postId)
        {
            Reaction = reaction;
            ReactorId = reactorId;
            PostId = postId;
        }
        public void Edit(ReactionTypes rt) { 
            this.Reaction = rt;
            this.UpdatedOn = DateTime.Now;
        }
        public void Delete() 
        { 
            this.IsDeleted = true;
            this.DeletedOn = DateTime.Now;
        }
    }
}
