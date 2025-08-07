using LinkifyBLL.Services.Abstraction;
using LinkifyDAL.Entities;
using LinkifyDAL.Repo.Abstraction;

namespace LinkifyBLL.Services.Implementation
{
    public class SharePostService : ISharePostService
    {
        private readonly ISharePostRepository _sharePostRepository;

        public SharePostService(ISharePostRepository sharePostRepository)
        {
            _sharePostRepository = sharePostRepository;
        }

        public async Task<SharePost> GetShareByIdAsync(int shareId)
        {
            return await _sharePostRepository.GetByIdAsync(shareId);
        }

        public async Task<SharePost> SharePostAsync(int postId, string userId, string? caption = null)
        {
            return await _sharePostRepository.SharePostAsync(postId, userId, caption);
        }

        public async Task UpdateShareCaptionAsync(int shareId, string newCaption)
        {
            await _sharePostRepository.UpdateCaptionAsync(shareId, newCaption);
        }

        public async Task ArchiveShareAsync(int shareId)
        {
            await _sharePostRepository.ArchiveAsync(shareId);
        }

        public async Task RestoreShareAsync(int shareId)
        {
            await _sharePostRepository.RestoreAsync(shareId);
        }

        public async Task<SharePost> GetUserShareOfPostAsync(int postId, string userId)
        {
            return await _sharePostRepository.GetUserSharePostAsync(postId, userId);
        }

        public async Task<IEnumerable<SharePost>> GetUserSharesAsync(string userId, bool includeArchived = false)
        {
            return await _sharePostRepository.GetSharesByUserAsync(userId, includeArchived);
        }

        public async Task<IEnumerable<SharePost>> GetPostSharesAsync(int postId, bool includeArchived = false)
        {
            return await _sharePostRepository.GetSharesOfPostAsync(postId, includeArchived);
        }

        public async Task<IEnumerable<SharePost>> GetUserArchivedSharesAsync(string userId)
        {
            return await _sharePostRepository.GetArchivedSharesByUserAsync(userId);
        }

        public async Task<bool> HasUserSharedPostAsync(int postId, string userId)
        {
            return await _sharePostRepository.HasUserSharedPostAsync(postId, userId);
        }

        public async Task<int> GetPostShareCountAsync(int postId)
        {
            return await _sharePostRepository.GetShareCountAsync(postId);
        }

        public async Task<int> GetUserShareCountAsync(string userId, bool includeArchived = false)
        {
            return await _sharePostRepository.GetUserShareCountAsync(userId, includeArchived);
        }
    }
}
