using LinkifyDAL.DataBase;
using LinkifyDAL.Entities;
using LinkifyDAL.Enums;
using LinkifyDAL.Repo.Abstraction;
using Microsoft.EntityFrameworkCore;

namespace LinkifyDAL.Repo.Implementation
{
    public class PostReactionsRepository : IPostReactionsRepository
    {
        private readonly LinkifyDbContext _context;

        public PostReactionsRepository(LinkifyDbContext context)
        {
            this._context = context;
        }

        public async Task AddReactionAsync(PostReactions reaction)
        {
            if (reaction == null)
                throw new ArgumentNullException(nameof(reaction));

            await _context.PostReactions.AddAsync(reaction);
            await _context.SaveChangesAsync();
        }
        public async Task<PostReactions> GetReactionAsync(int postId, string userId)
        {
            if (string.IsNullOrWhiteSpace(userId))
                throw new ArgumentNullException(nameof(userId));
            if (postId <= 0)
                throw new ArgumentException("Invalid Post ID");

            return await _context.PostReactions
                            .FirstOrDefaultAsync(r =>
                                r.PostId == postId &&
                                r.ReactorId == userId &&
                                !r.IsDeleted);
        }
        public async Task<int> GetReactionCountAsync(int postId, ReactionTypes? type = null)
        {
            if (postId <= 0)
                throw new ArgumentException("Invalid Post ID");

            var query = _context.PostReactions
                .Where(r => r.PostId == postId && !r.IsDeleted);

            if (type.HasValue)
            {
                query = query.Where(r => r.Reaction == type.Value);
            }

            return await query.CountAsync();
        }

        public async Task<IEnumerable<PostReactions>> GetReactionsByPostAsync(int postId, bool includeDeleted = false)
        {
            if (postId <= 0)
                throw new ArgumentException("Invalid Post ID", nameof(postId));

            var query = _context.PostReactions
                .Where(r => r.PostId == postId)
                .Include(r => r.Reactor)
                .AsQueryable();

            if (!includeDeleted)
            {
                query = query.Where(r => !r.IsDeleted);
            }

            return await query
                .OrderByDescending(r => r.CreatedOn)
                .ToListAsync();
        }

        public async Task<IEnumerable<PostReactions>> GetReactionsByUserAsync(string userId, bool includeDeleted = false)
        {
            if (string.IsNullOrWhiteSpace(userId))
                throw new ArgumentNullException(nameof(userId));
            var query = _context.PostReactions
                .Where(r => r.ReactorId == userId)
                .Include(r => r.Post)
                .AsQueryable();
            if (!includeDeleted)
            {
                query = query.Where(r => !r.IsDeleted);
            }
            return await query
                .OrderByDescending(r => r.CreatedOn)
                .ToListAsync();
        }

        public async Task<bool> HasUserReactedAsync(int postId, string userId)
        {
            if (string.IsNullOrWhiteSpace(userId))
                throw new ArgumentNullException(nameof(userId));
            if (postId <= 0)
                throw new ArgumentException("Invalid Post ID");

            return await _context.PostReactions
                .AnyAsync(r =>
                    r.PostId == postId &&
                    r.ReactorId == userId &&
                    !r.IsDeleted);
        }

        public async Task RemoveReactionAsync(int reactionId)
        {
            if (reactionId <= 0)
                throw new ArgumentException("Invalid Reaction ID");

            var reaction = await _context.PostReactions.FindAsync(reactionId);
            if (reaction != null)
            {
                reaction.Delete();
                await _context.SaveChangesAsync();
            }
        }

        public async Task UpdateReactionAsync(int reactionId, ReactionTypes newReactionType)
        {
            var reaction = await _context.PostReactions.FindAsync(reactionId);
            if (reaction == null) throw new ArgumentException("Reaction not found");

            reaction.Edit(newReactionType);
            await _context.SaveChangesAsync();
        }
    }
}
