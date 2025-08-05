using LinkifyDAL.DataBase;
using LinkifyDAL.Entities;
using LinkifyDAL.Repo.Abstraction;
using Microsoft.EntityFrameworkCore;

namespace LinkifyDAL.Repo.Implementation
{
    public class PostImagesRepository : IPostImagesRepository
    {
        private readonly LinkifyDbContext _context;
        public PostImagesRepository(LinkifyDbContext context)
        {
            this._context = context;
        }
        public async Task<PostImages> AddPostImageAsync(PostImages image)
        {
            if (image == null)
                throw new ArgumentNullException(nameof(image));

            await _context.PostImages.AddAsync(image);
            await _context.SaveChangesAsync();
            return image;
        }
        public async Task AddRangeAsync(IEnumerable<PostImages> images)
        {
            if (images == null || !images.Any())
                throw new ArgumentException("Images collection cannot be null or empty", nameof(images));

            await _context.PostImages.AddRangeAsync(images);
            await _context.SaveChangesAsync();
        }
        public async Task DeleteImageAsync(int imageId)
        {
            if (imageId <= 0)
                throw new ArgumentException("Invalid image ID", nameof(imageId));

            var image = await _context.PostImages.FindAsync(imageId);
            if (image != null)
            {
                image.Delete();
                await _context.SaveChangesAsync();
            }
        }
        public async Task<PostImages> GetImageByImageIdAsync(int imageId)
        {
            if (imageId <= 0)
                throw new ArgumentException("Invalid image ID", nameof(imageId));

            return await _context.PostImages
                .FirstOrDefaultAsync(img => img.Id == imageId);
        }
        public async Task<IEnumerable<PostImages>> GetDeletedImagesAsync(DateTime? since = null)
        {
            var query = _context.PostImages
                .Where(img => img.IsDeleted);

            if (since.HasValue)
            {
                query = query.Where(img => img.DeletedOn >= since.Value);
            }

            return await query
                .OrderByDescending(img => img.DeletedOn)
                .ToListAsync();
        }
        public async Task<IEnumerable<PostImages>> GetImageByPostIdAsync(int postId)
        {
            if (postId <= 0)
                throw new ArgumentException("Invalid post ID", nameof(postId));

            return await _context.PostImages
                .Where(img => img.PostId == postId && !img.IsDeleted)
                .OrderBy(img => img.CreatedOn)
                .ToListAsync();
        }
        public async Task<int> GetImageCountForPostAsync(int postId)
        {
            if (postId <= 0)
                throw new ArgumentException("Invalid post ID", nameof(postId));
            return await _context.PostImages
                .CountAsync(img => img.PostId == postId && !img.IsDeleted);
        }

        public async Task UpdateImagePathAsync(int imageId, string newPath)
        {
            if (imageId <= 0)
                throw new ArgumentException("Invalid image ID", nameof(imageId));
            if (string.IsNullOrWhiteSpace(newPath))
                throw new ArgumentNullException(nameof(newPath));

            var image = await _context.PostImages.FindAsync(imageId);
            if (image != null)
            {
                image.Update(newPath);
                await _context.SaveChangesAsync();
            }
        }
    }
}
