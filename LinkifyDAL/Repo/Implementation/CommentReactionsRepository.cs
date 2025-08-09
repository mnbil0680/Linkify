using LinkifyDAL.DataBase;
using LinkifyDAL.Entities;
using LinkifyDAL.Enums;
using LinkifyDAL.Repo.Abstraction;
using Microsoft.EntityFrameworkCore;

namespace LinkifyDAL.Repo.Implementation
{
    public class CommentReactionsRepository : ICommentReactionsRepository
    {
        private readonly LinkifyDbContext _context;

        public CommentReactionsRepository(LinkifyDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<CommentReactions> GetReactionAsync(int commentId, string userId)
        {
            return await _context.CommentReactions
                .FirstOrDefaultAsync(cr =>
                    cr.CommentId == commentId &&
                    cr.ReactorId == userId &&
                    !cr.IsDeleted);
        }

        public async Task AddReactionAsync(CommentReactions reaction)
        {
            _context.CommentReactions.Add(reaction);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateReactionAsync(int reactionId, ReactionTypes newReactionType)
        {
            var reaction = await _context.CommentReactions.FindAsync(reactionId);
            if (reaction != null)
            {
                reaction.Edit(newReactionType);
                await _context.SaveChangesAsync();
            }
        }

        public async Task RemoveReactionAsync(int reactionId)
        {
            var reaction = await _context.CommentReactions.FindAsync(reactionId);
            if (reaction != null)
            {
                reaction.Delete();
                await _context.SaveChangesAsync();
            }
        }

        public async Task<bool> HasUserReactedAsync(int commentId, string userId)
        {
            return await _context.CommentReactions
                .AnyAsync(cr =>
                    cr.CommentId == commentId &&
                    cr.ReactorId == userId &&
                    !cr.IsDeleted);
        }

        public async Task<int> GetReactionCountAsync(int commentId, ReactionTypes? type = null)
        {
            var query = _context.CommentReactions
                .Where(cr => cr.CommentId == commentId && !cr.IsDeleted);

            if (type.HasValue)
            {
                query = query.Where(cr => cr.Reaction == type.Value);
            }

            return await query.CountAsync();
        }

        public async Task<IEnumerable<CommentReactions>> GetReactionsByCommentAsync(int commentId, bool includeDeleted = false)
        {
            var query = _context.CommentReactions
                .Where(cr => cr.CommentId == commentId)
                .Include(cr => cr.Reactor)
                .AsQueryable();

            if (!includeDeleted)
            {
                query = query.Where(cr => !cr.IsDeleted);
            }

            return await query.ToListAsync();
        }

        public async Task<IEnumerable<CommentReactions>> GetReactionsByUserAsync(string userId, bool includeDeleted = false)
        {
            var query = _context.CommentReactions
                .Where(cr => cr.ReactorId == userId)
                .Include(cr => cr.Comment)
                .AsQueryable();

            if (!includeDeleted)
            {
                query = query.Where(cr => !cr.IsDeleted);
            }

            return await query.ToListAsync();
        }
    }
}
