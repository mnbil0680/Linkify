using LinkifyDAL.Entities;
using LinkifyDAL.Enums;

namespace LinkifyDAL.Repo.Abstraction
{
    public interface ICommentReactionsRepository
    {
        Task<CommentReactions> GetReactionAsync(int commentId, string userId);
        Task AddReactionAsync(CommentReactions reaction);
        Task UpdateReactionAsync(int reactionId, ReactionTypes newReactionType);
        Task RemoveReactionAsync(int reactionId);
        Task<bool> HasUserReactedAsync(int commentId, string userId);
        Task<int> GetReactionCountAsync(int commentId, ReactionTypes? type = null);
        Task<IEnumerable<CommentReactions>> GetReactionsByCommentAsync(int commentId, bool includeDeleted = false);
        Task<IEnumerable<CommentReactions>> GetReactionsByUserAsync(string userId, bool includeDeleted = false);
    }
}
