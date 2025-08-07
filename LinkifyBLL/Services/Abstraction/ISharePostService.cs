using LinkifyDAL.Entities;

namespace LinkifyBLL.Services.Abstraction
{
    public interface ISharePostService
    {
        Task<SharePost> GetShareByIdAsync(int shareId);
        Task<SharePost> SharePostAsync(int postId, string userId, string? caption = null);
        Task UpdateShareCaptionAsync(int shareId, string newCaption);
        Task ArchiveShareAsync(int shareId);
        Task RestoreShareAsync(int shareId);
        Task<SharePost> GetUserShareOfPostAsync(int postId, string userId);
        Task<IEnumerable<SharePost>> GetUserSharesAsync(string userId, bool includeArchived = false);
        Task<IEnumerable<SharePost>> GetPostSharesAsync(int postId, bool includeArchived = false);
        Task<IEnumerable<SharePost>> GetUserArchivedSharesAsync(string userId);
        Task<bool> HasUserSharedPostAsync(int postId, string userId);
        Task<int> GetPostShareCountAsync(int postId);
        Task<int> GetUserShareCountAsync(string userId, bool includeArchived = false);
    }
}
