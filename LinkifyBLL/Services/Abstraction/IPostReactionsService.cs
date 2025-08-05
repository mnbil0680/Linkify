using LinkifyDAL.Enums;

namespace LinkifyBLL.Services.Abstraction
{
    public interface IPostReactionsService
    {
        Task ToggleReactionAsync(int postId, string userId, ReactionTypes reactionType);
    }
}
