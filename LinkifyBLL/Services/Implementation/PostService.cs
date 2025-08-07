using LinkifyBLL.Services.Abstraction;
using LinkifyDAL.Entities;
using LinkifyDAL.Repo.Abstraction;

namespace LinkifyBLL.Services.Implementation
{
    public class PostService : IPostService
    {
        private readonly IPostRepository _postRepo;

        public PostService(IPostRepository postRepo)
        {
            _postRepo = postRepo;
        }

        public async Task<Post> GetPostByIdAsync(int postId)
        {
            if (postId <= 0)
                throw new ArgumentException("Invalid post ID");

            var post = await _postRepo.GetByIdAsync(postId);
            return post ?? throw new KeyNotFoundException("Post not found");
        }

        public async Task<Post> CreatePostAsync(string userId, string textContent)
        {
            if (string.IsNullOrWhiteSpace(userId))
                throw new ArgumentNullException(nameof(userId));
            if (string.IsNullOrWhiteSpace(textContent))
                throw new ArgumentNullException(nameof(textContent));

            var post = new Post(textContent, userId);
            return await _postRepo.CreateAsync(post);
        }

        public async Task UpdatePostAsync(int postId, string textContent)
        {
            if (string.IsNullOrWhiteSpace(textContent))
                throw new ArgumentNullException(nameof(textContent));

            var post = await _postRepo.GetByIdAsync(postId);
            if (post == null)
                throw new KeyNotFoundException("Post not found");

            post.Edit(textContent);
            await _postRepo.UpdateAsync(post);
        }

        public async Task DeletePostAsync(int postId)
        {
            if (!await _postRepo.ExistsAsync(postId))
                throw new KeyNotFoundException("Post not found");

            await _postRepo.DeleteAsync(postId);
        }

        public async Task<IEnumerable<Post>> GetUserPostsAsync(string userId)
        {
            if (string.IsNullOrWhiteSpace(userId))
                throw new ArgumentNullException(nameof(userId));

            return await _postRepo.GetByUserIdAsync(userId);
        }

        public async Task<int> GetUserPostCountAsync(string userId)
        {
            if (string.IsNullOrWhiteSpace(userId))
                throw new ArgumentNullException(nameof(userId));

            return await _postRepo.GetPostCountAsync(userId);
        }

        public async Task<IEnumerable<Post>> GetRecentPostsAsync(int count = 10)
        {
            if (count <= 0)
                throw new ArgumentException("Count must be positive", nameof(count));

            return await _postRepo.GetRecentPostsAsync(count);
        }

        public async Task<IEnumerable<Post>> GetPopularPostsAsync(TimeSpan since)
        {
            return await _postRepo.GetPopularPostsAsync(since);
        }

        public async Task<bool> IsPostOwnerAsync(int postId, string userId)
        {
            if (string.IsNullOrWhiteSpace(userId))
                throw new ArgumentNullException(nameof(userId));

            return await _postRepo.IsOwnerAsync(postId, userId);
        }
    }
}
