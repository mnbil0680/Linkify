using LinkifyDAL.Entities;

namespace LinkifyBLL.Services.Abstraction
{
    public interface IPostCommentsService
    {
        Task<PostComments> GetCommentAsync(int commentId);
        Task<PostComments> CreateCommentAsync(int postId, string commenterId, string content, string? imgPath = null, int? parentCommentId = null);
        Task UpdateCommentAsync(int commentId, string newContent, string? newImgPath = null);
        Task DeleteCommentAsync(int commentId);
        Task<PostComments> ReplyToCommentAsync(int parentCommentId, string commenterId, string content, string? imgPath = null);
        Task<IEnumerable<PostComments>> GetCommentsForPostAsync(int postId);
        Task<IEnumerable<PostComments>> GetCommentRepliesAsync(int parentCommentId);
        Task<bool> IsCommentOwnerAsync(int commentId, string userId);
        Task<int> GetCommentCountForPostAsync(int postId);
        //Write a 
    }
}
