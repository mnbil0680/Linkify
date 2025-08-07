using LinkifyDAL.Entities;

namespace LinkifyDAL.Repo.Abstraction
{
    public interface ISharePostRepository
    {
        Task<SharePost> GetByIdAsync(int shareId);
        Task<SharePost> SharePostAsync(int postId, string userId, string? caption = null);
        Task UpdateCaptionAsync(int shareId, string newCaption);
        Task ArchiveAsync(int shareId);
        Task RestoreAsync(int shareId);
        Task<SharePost> GetUserSharePostAsync(int postId, string userId);
        Task<IEnumerable<SharePost>> GetSharesByUserAsync(string userId, bool includeArchived = false);
        Task<IEnumerable<SharePost>> GetSharesOfPostAsync(int postId, bool includeArchived = false);
        Task<IEnumerable<SharePost>> GetArchivedSharesByUserAsync(string userId);
        Task<bool> HasUserSharedPostAsync(int postId, string userId);
        Task<int> GetShareCountAsync(int postId);
        Task<int> GetUserShareCountAsync(string userId, bool includeArchived = false);
    }
}
