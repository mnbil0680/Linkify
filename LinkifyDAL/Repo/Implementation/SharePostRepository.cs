using LinkifyDAL.DataBase;
using LinkifyDAL.Entities;
using LinkifyDAL.Repo.Abstraction;
using Microsoft.EntityFrameworkCore;

namespace LinkifyDAL.Repo.Implementation
{
    public class SharePostRepository : ISharePostRepository
    {
        private readonly LinkifyDbContext _context;

        public SharePostRepository(LinkifyDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<SharePost> GetByIdAsync(int shareId)
        {
            return await _context.SharePosts
                .Include(sp => sp.Post)
                .Include(sp => sp.User)
                .FirstOrDefaultAsync(sp => sp.Id == shareId);
        }

        public async Task<SharePost> SharePostAsync(int postId, string userId, string? caption = null)
        {
            var existingShare = await GetUserSharePostAsync(postId, userId);
            if (existingShare != null)
            {
                if (existingShare.IsArchived)
                {
                    existingShare.Restore();
                    existingShare.UpdateCaption(caption);
                    await _context.SaveChangesAsync();
                    return existingShare;
                }
                return existingShare;
            }

            var sharePost = new SharePost(postId, userId, caption);
            _context.SharePosts.Add(sharePost);
            await _context.SaveChangesAsync();
            return sharePost;
        }

        public async Task UpdateCaptionAsync(int shareId, string newCaption)
        {
            var sharePost = await GetByIdAsync(shareId);
            if (sharePost != null)
            {
                sharePost.UpdateCaption(newCaption);
                await _context.SaveChangesAsync();
            }
        }

        public async Task ArchiveAsync(int shareId)
        {
            var sharePost = await GetByIdAsync(shareId);
            if (sharePost != null)
            {
                sharePost.Archive();
                await _context.SaveChangesAsync();
            }
        }

        public async Task RestoreAsync(int shareId)
        {
            var sharePost = await GetByIdAsync(shareId);
            if (sharePost != null)
            {
                sharePost.Restore();
                await _context.SaveChangesAsync();
            }
        }

        public async Task<SharePost> GetUserSharePostAsync(int postId, string userId)
        {
            return await _context.SharePosts
                .FirstOrDefaultAsync(sp => sp.PostId == postId && sp.UserId == userId);
        }

        public async Task<IEnumerable<SharePost>> GetSharesByUserAsync(string userId, bool includeArchived = false)
        {
            var query = _context.SharePosts
                .Include(sp => sp.Post)
                .Where(sp => sp.UserId == userId);

            if (!includeArchived)
            {
                query = query.Where(sp => !sp.IsArchived);
            }

            return await query.ToListAsync();
        }

        public async Task<IEnumerable<SharePost>> GetSharesOfPostAsync(int postId, bool includeArchived = false)
        {
            var query = _context.SharePosts
                .Include(sp => sp.User)
                .Where(sp => sp.PostId == postId);

            if (!includeArchived)
            {
                query = query.Where(sp => !sp.IsArchived);
            }

            return await query.ToListAsync();
        }

        public async Task<IEnumerable<SharePost>> GetArchivedSharesByUserAsync(string userId)
        {
            return await _context.SharePosts
                .Include(sp => sp.Post)
                .Where(sp => sp.UserId == userId && sp.IsArchived)
                .ToListAsync();
        }

        public async Task<bool> HasUserSharedPostAsync(int postId, string userId)
        {
            return await _context.SharePosts
                .AnyAsync(sp => sp.PostId == postId && sp.UserId == userId && !sp.IsArchived);
        }

        public async Task<int> GetShareCountAsync(int postId)
        {
            return await _context.SharePosts
                .CountAsync(sp => sp.PostId == postId && !sp.IsArchived);
        }

        public async Task<int> GetUserShareCountAsync(string userId, bool includeArchived = false)
        {
            var query = _context.SharePosts
                .Where(sp => sp.UserId == userId);

            if (!includeArchived)
            {
                query = query.Where(sp => !sp.IsArchived);
            }

            return await query.CountAsync();
        }
    }
}
