using LinkifyDAL.DataBase;
using LinkifyDAL.Entities;
using LinkifyDAL.Repo.Abstraction;
using Microsoft.EntityFrameworkCore;

namespace LinkifyDAL.Repo.Implementation
{
    public class SavePostRepository : ISavePostRepository
    {
        private readonly LinkifyDbContext _context;
        public SavePostRepository(LinkifyDbContext context)
        {
            this._context = context;
        }
        public async Task<SavePost> GetByIdAsync(int savedPostId)
        {
            return await _context.SavePosts
                .Include(sp => sp.Post)
                .Include(sp => sp.User)
                .FirstOrDefaultAsync(sp => sp.PostId == savedPostId);
        }
        public async Task<SavePost> SavePostAsync(int postId, string userId)
        {
            var existingSave = await GetUserSavedPostAsync(postId, userId);
            if (existingSave != null)
            {
                if (existingSave.IsArchived)
                {
                    existingSave.Restore();
                    await _context.SaveChangesAsync();
                    return existingSave;
                }
                return existingSave;
            }
            var savePost = new SavePost(postId, userId);
            _context.SavePosts.Add(savePost);
            await _context.SaveChangesAsync();
            return savePost;
        }
        public async Task ArchiveAsync(int savedPostId)
        {
            var savePost = await GetByIdAsync(savedPostId);
            if (savePost != null)
            {
                savePost.Archive();
                await _context.SaveChangesAsync();
            }
        }
        public async Task RestoreAsync(int savedPostId)
        {
            var savePost = await GetByIdAsync(savedPostId);
            if (savePost != null)
            {
                savePost.Restore();
                await _context.SaveChangesAsync();
            }
        }
        public async Task<SavePost> GetUserSavedPostAsync(int postId, string userId)
        {
            return await _context.SavePosts
                .FirstOrDefaultAsync(sp => sp.PostId == postId && sp.UserId == userId);
        }
        public async Task<IEnumerable<SavePost>> GetSavedPostsByUserIdAsync(string userId, bool includeArchived = false)
        {
            var query = _context.SavePosts
                .Include(sp => sp.Post)
                .Where(sp => sp.UserId == userId);

            if (!includeArchived)
            {
                query = query.Where(sp => !sp.IsArchived);
            }

            return await query.ToListAsync();
        }
        public async Task<IEnumerable<SavePost>> GetArchivedSavedPostsAsync(string userId)
        {
            return await _context.SavePosts
                .Include(sp => sp.Post)
                .Where(sp => sp.UserId == userId && sp.IsArchived)
                .ToListAsync();
        }
        public async Task<bool> IsPostSavedByUserAsync(int postId, string userId)
        {
            return await _context.SavePosts
                .AnyAsync(sp => sp.PostId == postId && sp.UserId == userId && !sp.IsArchived);
        }
        public async Task<bool> IsPostArchivedByUserAsync(int postId, string userId)
        {
            return await _context.SavePosts
                .AnyAsync(sp => sp.PostId == postId && sp.UserId == userId && sp.IsArchived);
        }
        public async Task<int> GetSavedPostCountAsync(int postId)
        {
            return await _context.SavePosts
                .CountAsync(sp => sp.PostId == postId && !sp.IsArchived);
        }
        public async Task<int> GetUserSavedPostCountAsync(string userId, bool includeArchived = false)
        {
            var query = _context.SavePosts
                .Where(sp => sp.UserId == userId);
            if (!includeArchived)
            {
                query = query.Where(sp => !sp.IsArchived);
            }
            return await query.CountAsync();
        }
    }
}
