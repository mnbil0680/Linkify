using LinkifyBLL.Services.Abstraction;
using LinkifyDAL.Entities;
using LinkifyDAL.Repo.Abstraction;

namespace LinkifyBLL.Services.Implementation
{
    public class PostImagesService : IPostImagesService
    {
        private readonly IPostImagesRepository _imagesRepository;

        public PostImagesService(IPostImagesRepository imagesRepository)
        {
            _imagesRepository = imagesRepository ?? throw new ArgumentNullException(nameof(imagesRepository));
        }

        public async Task<PostImages> GetImageByImageIdAsync(int imageId)
        {
            if (imageId <= 0)
                throw new ArgumentException("Image ID must be positive", nameof(imageId));

            var image = await _imagesRepository.GetImageByImageIdAsync(imageId);
            return image ?? throw new KeyNotFoundException("Image not found");
        }

        public async Task<IEnumerable<PostImages>> GetImageByPostIdAsync(int postId)
        {
            if (postId <= 0)
                throw new ArgumentException("Post ID must be positive", nameof(postId));

            return await _imagesRepository.GetImageByPostIdAsync(postId);
        }

        public async Task<PostImages> AddPostImageAsync(PostImages image)
        {
            if (image == null)
                throw new ArgumentNullException(nameof(image));

            if (string.IsNullOrWhiteSpace(image.ImagePath))
                throw new ArgumentException("Image path cannot be empty", nameof(image.ImagePath));

            if (image.PostId <= 0)
                throw new ArgumentException("Invalid Post ID", nameof(image.PostId));

            return await _imagesRepository.AddPostImageAsync(image);
        }

        public async Task UpdateImagePathAsync(int imageId, string newPath)
        {
            if (imageId <= 0)
                throw new ArgumentException("Image ID must be positive", nameof(imageId));

            if (string.IsNullOrWhiteSpace(newPath))
                throw new ArgumentException("New path cannot be empty", nameof(newPath));

            await _imagesRepository.UpdateImagePathAsync(imageId, newPath);
        }

        public async Task DeleteImageAsync(int imageId)
        {
            if (imageId <= 0)
                throw new ArgumentException("Image ID must be positive", nameof(imageId));

            await _imagesRepository.DeleteImageAsync(imageId);
        }

        public async Task AddRangeAsync(IEnumerable<PostImages> images)
        {
            if (images == null)
                throw new ArgumentNullException(nameof(images));

            if (!images.Any())
                throw new ArgumentException("Image collection cannot be empty", nameof(images));

            await _imagesRepository.AddRangeAsync(images);
        }

        public async Task<IEnumerable<PostImages>> GetDeletedImagesAsync(DateTime? since = null)
        {
            return await _imagesRepository.GetDeletedImagesAsync(since);
        }

        public async Task<int> GetImageCountForPostAsync(int postId)
        {
            if (postId <= 0)
                throw new ArgumentException("Post ID must be positive", nameof(postId));

            return await _imagesRepository.GetImageCountForPostAsync(postId);
        }
    }
}
