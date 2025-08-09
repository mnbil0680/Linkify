using LinkifyDAL.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace LinkifyDAL.Entities
{
    public class CommentReactions
    {
        public int Id { get; private set; }
        public ReactionTypes Reaction { get; private set; }
        public string ReactorId { get; private set; }
        public int CommentId { get; private set; }
        public bool IsDeleted { get; private set; } = false;
        public DateTime CreatedOn { get; private set; } = DateTime.Now;
        public DateTime? UpdatedOn { get; private set; }
        public DateTime? DeletedOn { get; private set; }

        [ForeignKey(nameof(ReactorId))]
        public User Reactor { get; private set; }

        [ForeignKey(nameof(CommentId))]
        public PostComments Comment { get; private set; }
        public CommentReactions(ReactionTypes reaction, string reactorId, int commentId)
        {
            Reaction = reaction;
            ReactorId = reactorId;
            CommentId = commentId;
        }
        public void Edit(ReactionTypes newReaction)
        {
            Reaction = newReaction;
            UpdatedOn = DateTime.Now;
        }
        public void Delete()
        {
            IsDeleted = true;
            DeletedOn = DateTime.Now;
        }
    }
}
