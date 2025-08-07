using LinkifyBLL.Services.Abstraction;
using LinkifyDAL.Entities;
using LinkifyDAL.Repo.Abstraction;

namespace LinkifyBLL.Services.Implementation
{
    public class SavePostService : ISavePostService
    {
        private readonly ISavePostRepository _savePostRepository;
        public SavePostService(ISavePostRepository savePostRepository)
        {
            _savePostRepository = savePostRepository;
        }
        public async Task ArchiveAsync(int savedPostId)
        {
            await _savePostRepository.ArchiveAsync(savedPostId);
        }

        public async Task<IEnumerable<SavePost>> GetArchivedSavedPostsAsync(string userId)
        {
            return await _savePostRepository.GetArchivedSavedPostsAsync(userId);
        }

        public async Task<SavePost> GetByIdAsync(int savedPostId)
        {
            return await _savePostRepository.GetByIdAsync(savedPostId);
        }

        public async Task<int> GetSavedPostCountAsync(int postId)
        {
            return await _savePostRepository.GetSavedPostCountAsync(postId);
        }

        public async Task<IEnumerable<SavePost>> GetSavedPostsByUserIdAsync(string userId, bool includeArchived = false)
        {
            return await _savePostRepository.GetSavedPostsByUserIdAsync(userId, includeArchived);
        }

        public async Task<SavePost> GetUserSavedPostAsync(int postId, string userId)
        {
            return await _savePostRepository.GetUserSavedPostAsync(postId, userId);
        }

        public async Task<int> GetUserSavedPostCountAsync(string userId, bool includeArchived = false)
        {
            return await _savePostRepository.GetUserSavedPostCountAsync(userId, includeArchived);
        }

        public async Task<bool> IsPostArchivedByUserAsync(int postId, string userId)
        {
            return await _savePostRepository.IsPostArchivedByUserAsync(postId, userId);
        }

        public async Task<bool> IsPostSavedByUserAsync(int postId, string userId)
        {
            return await _savePostRepository.IsPostSavedByUserAsync(postId, userId);
        }

        public async Task RestoreAsync(int savedPostId)
        {
            await _savePostRepository.RestoreAsync(savedPostId);
        }

        public async Task<SavePost> SavePostAsync(int postId, string userId)
        {
            return await _savePostRepository.SavePostAsync(postId, userId);
        }
    }
}
