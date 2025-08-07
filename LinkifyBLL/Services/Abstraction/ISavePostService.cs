using LinkifyDAL.Entities;

namespace LinkifyBLL.Services.Abstraction
{
    public interface ISavePostService
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

        //how many times the post is saved
        Task<int> GetSavedPostCountAsync(int postId);

        //how many posts the user has saved
        Task<int> GetUserSavedPostCountAsync(string userId, bool includeArchived = false);
    }
}
