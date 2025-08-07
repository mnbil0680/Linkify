using LinkifyDAL.Entities;

namespace LinkifyBLL.Services.Abstraction
{
    public interface IPostService
    {
        Task<Post> GetPostByIdAsync(int postId);
        Task<Post> CreatePostAsync(string userId, string textContent);
        Task UpdatePostAsync(int postId, string textContent);
        Task DeletePostAsync(int postId);
        Task<IEnumerable<Post>> GetUserPostsAsync(string userId);
        Task<int> GetUserPostCountAsync(string userId);
        Task<IEnumerable<Post>> GetRecentPostsAsync(int count = 10);
        Task<IEnumerable<Post>> GetPopularPostsAsync(TimeSpan since);
        Task<bool> IsPostOwnerAsync(int postId, string userId);
    }
}
