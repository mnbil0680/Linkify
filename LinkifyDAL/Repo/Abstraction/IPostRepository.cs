using LinkifyDAL.Entities;

namespace LinkifyDAL.Repo.Abstraction
{
    public interface IPostRepository
    {
        Task<Post> GetByIdAsync(int postId);
        Task<Post> CreateAsync(Post post);
        Task UpdateAsync(Post post);
        Task DeleteAsync(int postId);
        Task<IEnumerable<Post>> GetByUserIdAsync(string userId);
        Task<IEnumerable<Post>> GetRecentPostsAsync(int count = 10);
        Task<IEnumerable<Post>> GetPopularPostsAsync(TimeSpan since);
        Task<bool> ExistsAsync(int postId);
        Task<bool> IsOwnerAsync(int postId, string userId);
        Task<int> GetPostCountAsync(string userId = null);
        Task<IEnumerable<Post>> GetDeletedPostsAsync();
    }
}
