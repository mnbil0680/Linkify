using LinkifyDAL.Entities;

namespace LinkifyDAL.Repo.Abstraction
{
    public interface IPostImagesRepository
    {
        Task<PostImages> GetImageByImageIdAsync(int imageId);
        Task<IEnumerable<PostImages>> GetImageByPostIdAsync(int postId);
        Task<PostImages> AddPostImageAsync(PostImages image);
        Task UpdateImagePathAsync(int imageId, string newPath);
        Task DeleteImageAsync(int imageId);
        Task AddRangeAsync(IEnumerable<PostImages> images);
        Task<IEnumerable<PostImages>> GetDeletedImagesAsync(DateTime? since = null);
        Task<int> GetImageCountForPostAsync(int postId);
    }
}
