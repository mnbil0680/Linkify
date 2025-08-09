using LinkifyDAL.Entities;
using LinkifyDAL.Enums;

namespace LinkifyBLL.Services.Abstraction
{
    public interface ICommentReactionsService
    {
        Task ToggleReactionAsync(int commentId, string userId, ReactionTypes reactionType);
        Task<IEnumerable<CommentReactions>> GetReactionsByCommentAsync(int commentId, bool includeDeleted = false);
        Task<IEnumerable<CommentReactions>> GetReactionsByUserAsync(string userId, bool includeDeleted = false);
        Task<int> GetReactionCountAsync(int commentId, ReactionTypes? type = null);
    }
}
