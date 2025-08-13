using LinkifyBLL.Services.Abstraction;
using LinkifyDAL.Entities;
using LinkifyDAL.Repo.Abstraction;

namespace LinkifyBLL.Services.Implementation
{
    public class PostCommentsService : IPostCommentsService
    {
        private readonly IPostCommentsRepository _commentsRepo;
        private readonly IPostRepository _postRepo;

        public PostCommentsService(IPostCommentsRepository commentsRepo, IPostRepository postRepo)
        {
            _commentsRepo = commentsRepo;
            _postRepo = postRepo;
        }

        public async Task<PostComments> GetCommentAsync(int commentId)
        {
            if (commentId <= 0)
                throw new ArgumentException("Invalid comment ID");

            return await _commentsRepo.GetCommentByIdAsync(commentId)
                ?? throw new KeyNotFoundException("Comment not found");
        }

        public async Task<PostComments> CreateCommentAsync(
            int postId,
            string commenterId,
            string content,
            string? imgPath = null,
            int? parentCommentId = null)
        {
            if (postId <= 0)
                throw new ArgumentException("Invalid post ID");
            if (string.IsNullOrWhiteSpace(commenterId))
                throw new ArgumentNullException(nameof(commenterId));
            if (string.IsNullOrWhiteSpace(content))
                throw new ArgumentNullException(nameof(content));

            if (!await _postRepo.ExistsAsync(postId))
                throw new KeyNotFoundException("Post not found");

            if (parentCommentId.HasValue && !await _commentsRepo.ExistsAsync(parentCommentId.Value))
                throw new KeyNotFoundException("Parent comment not found");

            var comment = new PostComments(content, postId, commenterId, imgPath, parentCommentId);
            return await _commentsRepo.AddAsync(comment);
        }

        public async Task UpdateCommentAsync(int commentId, string newContent, string? newImgPath = null)
        {
            if (string.IsNullOrWhiteSpace(newContent))
                throw new ArgumentNullException(nameof(newContent));

            await _commentsRepo.UpdateAsync(commentId, newContent, newImgPath);
        }

        public async Task DeleteCommentAsync(int commentId)
        {
            await _commentsRepo.DeleteAsync(commentId);
        }

        public async Task<PostComments> ReplyToCommentAsync(
            int parentCommentId,
            string commenterId,
            string content,
            string? imgPath = null)
        {
            var parentComment = await GetCommentAsync(parentCommentId);
            return await CreateCommentAsync(
                parentComment.PostId,
                commenterId,
                content,
                imgPath,
                parentCommentId);
        }

        public async Task<IEnumerable<PostComments>> GetCommentsForPostAsync(int postId)
        {
            if (postId <= 0)
                throw new ArgumentException("Invalid post ID");

            return await _commentsRepo.GetCommentsByPostIdAsync(postId);
        }

        public async Task<IEnumerable<PostComments>> GetCommentRepliesAsync(int parentCommentId)
        {
            if (parentCommentId <= 0)
                throw new ArgumentException("Invalid parent comment ID");

            return await _commentsRepo.GetRepliesAsync(parentCommentId);
        }

        public async Task<bool> IsCommentOwnerAsync(int commentId, string userId)
        {
            if (string.IsNullOrWhiteSpace(userId))
                throw new ArgumentNullException(nameof(userId));

            return await _commentsRepo.IsCommenterAsync(commentId, userId);
        }

        public async Task<int> GetCommentCountForPostAsync(int postId)
        {
            if (postId <= 0)
                throw new ArgumentException("Invalid post ID");

            return await _commentsRepo.GetCommentCountForPostAsync(postId);
        }


    }
}
