using LinkifyDAL.DataBase;
using LinkifyDAL.Entities;
using LinkifyDAL.Repo.Abstraction;
using Microsoft.EntityFrameworkCore;

namespace LinkifyDAL.Repo.Implementation
{
    public class PostCommentsRepository : IPostCommentsRepository
    {
        private readonly LinkifyDbContext _context;
        public PostCommentsRepository(LinkifyDbContext context)
        {
            this._context = context;
        }

        public async Task<PostComments> AddAsync(PostComments comment)
        {
            if (comment == null)
                throw new ArgumentNullException(nameof(comment));
            await _context.PostComments.AddAsync(comment);
            await _context.SaveChangesAsync();
            return comment;
        }
        public async Task DeleteAsync(int commentId)
        {
            if (commentId <= 0)
                throw new ArgumentException("Invalid comment ID", nameof(commentId));

            var comment = await _context.PostComments.FindAsync(commentId);
            if (comment != null)
            {
                comment.Delete();
                await _context.SaveChangesAsync();
            }
        }
        public async Task<bool> ExistsAsync(int commentId)
        {
            if (commentId <= 0)
                return false;
            return await _context.PostComments
                .AnyAsync(c => c.Id == commentId);
        }

        public async Task<PostComments> GetCommentByIdAsync(int commentId)
        {
            if (commentId <= 0)
                throw new ArgumentException("Invalid comment ID", nameof(commentId));
            return await _context.PostComments
                .Include(c => c.User)
                .FirstOrDefaultAsync(c => c.Id == commentId);
        }

        public async Task<IEnumerable<PostComments>> GetCommentsByPostIdAsync(int postId, bool includeDeleted = false)
        {
            if (postId <= 0)
                throw new ArgumentException("Invalid post ID", nameof(postId));
            var query = _context.PostComments
                .Include(c => c.User)
                .Where(c => c.PostId == postId && c.ParentCommentId == null);
            if (!includeDeleted)
            {
                query = query.Where(c => !c.IsDeleted);
            }
            return await query
                .OrderByDescending(c => c.CreatedOn)
                .ToListAsync();
        }

        public async Task<int> GetCommentCountForPostAsync(int postId, bool includeDeleted = false)
        {
            if (postId <= 0)
                throw new ArgumentException("Invalid post ID", nameof(postId));
            var query = _context.PostComments
                .Where(c => c.PostId == postId);
            if (!includeDeleted)
            {
                query = query.Where(c => !c.IsDeleted);
            }
            return await query.CountAsync();
        }

        public async Task<IEnumerable<PostComments>> GetRepliesAsync(int parentCommentId, bool includeDeleted = false)
        {
            if (parentCommentId <= 0)
                throw new ArgumentException("Invalid parent comment ID", nameof(parentCommentId));
            var query = _context.PostComments
                .Include(c => c.User)
                .Where(c => c.ParentCommentId == parentCommentId);
            if (!includeDeleted)
            {
                query = query.Where(c => !c.IsDeleted);
            }
            return await query
                .OrderBy(c => c.CreatedOn)
                .ToListAsync();
        }
        public async Task<int> GetReplyCountAsync(int parentCommentId, bool includeDeleted = false)
        {
            if (parentCommentId <= 0)
                throw new ArgumentException("Invalid parent comment ID", nameof(parentCommentId));
            var query = _context.PostComments
                .Where(c => c.ParentCommentId == parentCommentId);
            if (!includeDeleted)
            {
                query = query.Where(c => !c.IsDeleted);
            }
            return await query.CountAsync();
        }

        public async Task<bool> IsCommenterAsync(int commentId, string userId)
        {
            if (commentId <= 0)
                throw new ArgumentException("Invalid comment ID", nameof(commentId));
            if (string.IsNullOrWhiteSpace(userId))
                throw new ArgumentNullException(nameof(userId));
            return await _context.PostComments
                .AnyAsync(c => c.Id == commentId && c.CommenterId == userId);
        }

        public async Task UpdateAsync(int commentId, string newContent, string? imgPath = null)
        {
            if (commentId <= 0)
                throw new ArgumentException("Invalid comment ID");
            if (newContent == null)
                throw new ArgumentNullException(nameof(newContent));

            var comment = await _context.PostComments.FindAsync(commentId);
            if (comment == null)
                throw new KeyNotFoundException("Comment not found");

            comment.Update(newContent, imgPath);
            await _context.SaveChangesAsync();
        }
    }
}
