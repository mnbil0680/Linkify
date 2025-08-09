using LinkifyBLL.Services.Abstraction;
using LinkifyDAL.Entities;
using LinkifyDAL.Enums;
using LinkifyDAL.Repo.Abstraction;

namespace LinkifyBLL.Services.Implementation
{
    public class CommentReactionsService : ICommentReactionsService
    {
        private readonly ICommentReactionsRepository _repository;

        public CommentReactionsService(ICommentReactionsRepository repository)
        {
            this._repository = repository;
        }

        public async Task ToggleReactionAsync(int commentId, string userId, ReactionTypes reactionType)
        {
            if (string.IsNullOrWhiteSpace(userId))
                throw new ArgumentNullException(nameof(userId));
            if (commentId <= 0)
                throw new ArgumentException("Invalid Comment ID");
            if (!Enum.IsDefined(typeof(ReactionTypes), reactionType))
                throw new ArgumentException("Invalid reaction type");

            var existing = await _repository.GetReactionAsync(commentId, userId);
            if (existing != null)
            {
                if (existing.Reaction == reactionType)
                {
                    // Remove if same reaction clicked
                    await _repository.RemoveReactionAsync(existing.Id);
                }
                else
                {
                    // Update if different reaction
                    await _repository.UpdateReactionAsync(existing.Id, reactionType);
                }
            }
            else
            {
                // Add new reaction
                await _repository.AddReactionAsync(new CommentReactions(reactionType, userId, commentId));
            }
        }

        public async Task<IEnumerable<CommentReactions>> GetReactionsByCommentAsync(int commentId, bool includeDeleted = false)
        {
            if (commentId <= 0)
                throw new ArgumentException("Invalid Comment ID");

            return await _repository.GetReactionsByCommentAsync(commentId, includeDeleted);
        }

        public async Task<IEnumerable<CommentReactions>> GetReactionsByUserAsync(string userId, bool includeDeleted = false)
        {
            if (string.IsNullOrWhiteSpace(userId))
                throw new ArgumentNullException(nameof(userId));

            return await _repository.GetReactionsByUserAsync(userId, includeDeleted);
        }

        public async Task<int> GetReactionCountAsync(int commentId, ReactionTypes? type = null)
        {
            if (commentId <= 0)
                throw new ArgumentException("Invalid Comment ID");

            return await _repository.GetReactionCountAsync(commentId, type);
        }
    }
}
