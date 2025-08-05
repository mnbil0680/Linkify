using LinkifyDAL.DataBase;
using LinkifyDAL.Entities;
using LinkifyDAL.Repo.Abstraction;
using Microsoft.EntityFrameworkCore;

namespace LinkifyDAL.Repo.Implementation
{
    public class PostRepository : IPostRepository
    {
        private readonly LinkifyDbContext _context;

        public PostRepository(LinkifyDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<Post> GetByIdAsync(int postId)
        {
            if (postId <= 0)
                throw new ArgumentException("Invalid post ID", nameof(postId));

            return await _context.Post
                .Include(p => p.User)
                .FirstOrDefaultAsync(p => p.Id == postId && !p.IsDeleted);
        }

        public async Task<Post> CreateAsync(Post post)
        {
            if (post == null)
                throw new ArgumentNullException(nameof(post));

            await _context.Post.AddAsync(post);
            await _context.SaveChangesAsync();
            return post;
        }

        public async Task UpdateAsync(Post post)
        {
            if (post == null)
                throw new ArgumentNullException(nameof(post));

            var existing = await _context.Post.FindAsync(post.Id);
            if (existing != null)
            {
                existing.Edit(post.TextContent);
                await _context.SaveChangesAsync();
            }
        }

        public async Task DeleteAsync(int postId)
        {
            if (postId <= 0)
                throw new ArgumentException("Invalid post ID", nameof(postId));

            var post = await _context.Post.FindAsync(postId);
            if (post != null)
            {
                post.Delete();
                await _context.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<Post>> GetByUserIdAsync(string userId)
        {
            if (string.IsNullOrWhiteSpace(userId))
                throw new ArgumentNullException(nameof(userId));

            return await _context.Post
                .Include(p => p.User)
                .Where(p => p.UserId == userId && !p.IsDeleted)
                .OrderByDescending(p => p.CreatedOn)
                .ToListAsync();
        }

        public async Task<IEnumerable<Post>> GetRecentPostsAsync(int count = 10)
        {
            if (count <= 0)
                throw new ArgumentException("Count must be positive", nameof(count));

            return await _context.Post
                .Include(p => p.User)
                .Where(p => !p.IsDeleted)
                .OrderByDescending(p => p.CreatedOn)
                .Take(count)
                .ToListAsync();
        }

        public async Task<IEnumerable<Post>> GetPopularPostsAsync(TimeSpan since)
        {
            var cutoff = DateTime.Now.Subtract(since);

            return await _context.Post
                .Include(p => p.User)
                .Where(p => !p.IsDeleted && p.CreatedOn >= cutoff)
                .OrderByDescending(p => p.CreatedOn) // Default ordering when no engagement metrics
                .ToListAsync();
        }

        public async Task<bool> ExistsAsync(int postId)
        {
            if (postId <= 0)
                return false;

            return await _context.Post
                .AnyAsync(p => p.Id == postId && !p.IsDeleted);
        }

        public async Task<bool> IsOwnerAsync(int postId, string userId)
        {
            if (string.IsNullOrWhiteSpace(userId))
                throw new ArgumentNullException(nameof(userId));

            return await _context.Post
                .AnyAsync(p => p.Id == postId && p.UserId == userId);
        }

        public async Task<int> GetPostCountAsync(string userId = null)
        {
            var query = _context.Post.Where(p => !p.IsDeleted);

            if (userId != null)
            {
                query = query.Where(p => p.UserId == userId);
            }

            return await query.CountAsync();
        }

        public async Task<IEnumerable<Post>> GetDeletedPostsAsync()
        {
            return await _context.Post
                .Include(p => p.User)
                .Where(p => p.IsDeleted)
                .OrderByDescending(p => p.DeletedOn)
                .ToListAsync();
        }
    }
}
