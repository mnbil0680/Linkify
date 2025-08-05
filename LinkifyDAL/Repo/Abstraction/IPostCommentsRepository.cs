using LinkifyDAL.Entities;
namespace LinkifyDAL.Repo.Abstraction
{
    public interface IPostCommentsRepository
    {
        Task<PostComments> GetCommentByIdAsync(int commentId);
        Task<PostComments> AddAsync(PostComments comment);
        Task UpdateAsync(int commentId, string newContent, string? imgPath = null);
        Task DeleteAsync(int commentId);
        Task<IEnumerable<PostComments>> GetCommentsByPostIdAsync(int postId, bool includeDeleted = false);
        Task<IEnumerable<PostComments>> GetRepliesAsync(int parentCommentId, bool includeDeleted = false);
        Task<bool> ExistsAsync(int commentId);
        Task<bool> IsCommenterAsync(int commentId, string userId);
        Task<int> GetCommentCountForPostAsync(int postId, bool includeDeleted = false);
        Task<int> GetReplyCountAsync(int parentCommentId, bool includeDeleted = false);
    }
}
