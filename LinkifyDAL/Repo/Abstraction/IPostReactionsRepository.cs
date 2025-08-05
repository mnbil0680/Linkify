using LinkifyDAL.Entities;
using LinkifyDAL.Enums;

namespace LinkifyDAL.Repo.Abstraction
{
    public interface IPostReactionsRepository
    {
        Task<PostReactions> GetReactionAsync(int postId, string userId);
        Task AddReactionAsync(PostReactions reaction);
        Task UpdateReactionAsync(int reactionId, ReactionTypes newReactionType);
        Task RemoveReactionAsync(int reactionId);
        Task<bool> HasUserReactedAsync(int postId, string userId);
        Task<int> GetReactionCountAsync(int postId, ReactionTypes? type = null);
        Task<IEnumerable<PostReactions>> GetReactionsByPostAsync(int postId, bool includeDeleted = false);
        Task<IEnumerable<PostReactions>> GetReactionsByUserAsync(string userId, bool includeDeleted = false);
    }
}
