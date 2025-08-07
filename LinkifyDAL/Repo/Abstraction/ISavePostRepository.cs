using LinkifyDAL.Entities;

namespace LinkifyDAL.Repo.Abstraction
{
    public interface ISavePostRepository
    {
        Task<SavePost> GetByIdAsync(int savedPostId);
        Task<SavePost> SavePostAsync(int postId, string userId);
        Task ArchiveAsync(int savedPostId);
        Task RestoreAsync(int savedPostId);
        Task<SavePost> GetUserSavedPostAsync(int postId, string userId);
        Task<IEnumerable<SavePost>> GetSavedPostsByUserIdAsync(string userId, bool includeArchived = false);
        Task<IEnumerable<SavePost>> GetArchivedSavedPostsAsync(string userId);
        Task<bool> IsPostSavedByUserAsync(int postId, string userId);
        Task<bool> IsPostArchivedByUserAsync(int postId, string userId);
        Task<int> GetSavedPostCountAsync(int postId);
        Task<int> GetUserSavedPostCountAsync(string userId, bool includeArchived = false);
    }
}
