using LinkifyDAL.Entities;
using LinkifyDAL.Enums;

namespace LinkifyBLL.Services.Abstraction
{
    public interface IPostReactionsService
    {
        Task ToggleReactionAsync(int postId, string userId, ReactionTypes reactionType);
        Task<IEnumerable<PostReactions>> GetReactionsByPostAsync(int postId, bool includeDeleted = false);
        Task<IEnumerable<PostReactions>> GetReactionsByUserAsync(string userId, bool includeDeleted = false);
        Task<int> GetReactionCountAsync(int postId, ReactionTypes? type = null);
    }
}
