using LinkifyBLL.Services.Abstraction;
using LinkifyDAL.Entities;
using LinkifyDAL.Enums;
using LinkifyDAL.Repo.Abstraction;

namespace LinkifyBLL.Services.Implementation
{
    public class PostReactionsService : IPostReactionsService
    {
        private readonly IPostReactionsRepository _repository;

        public PostReactionsService(IPostReactionsRepository repository)
        {
            _repository = repository;
        }

        public async Task ToggleReactionAsync(int postId, string userId, ReactionTypes reactionType)
        {
            if (string.IsNullOrWhiteSpace(userId))
                throw new ArgumentNullException(nameof(userId));
            if (postId <= 0)
                throw new ArgumentException("Invalid Post ID");
            if (!Enum.IsDefined(typeof(ReactionTypes), reactionType))
                throw new ArgumentException("Invalid reaction type");

            var existing = await _repository.GetReactionAsync(postId, userId);
            if (existing != null)
            {
                if (existing.Reaction == reactionType)
                {
                    // User clicked same reaction - remove it
                    await _repository.RemoveReactionAsync(existing.Id);
                }
                else
                {
                    // User changed reaction type - update it
                    await _repository.UpdateReactionAsync(existing.Id, reactionType);
                }
            }
            else
            {
                // Add new reaction
                await _repository.AddReactionAsync(new PostReactions(reactionType, userId, postId));
            }
        }
    }
}
