using LinkifyBLL.ModelView;
using LinkifyBLL.Services.Abstraction;
using LinkifyDAL.Entities;
using LinkifyDAL.Repo.Abstraction;
using Microsoft.AspNetCore.Http;
using SempaBLL.Helper;

namespace LinkifyBLL.Services.Implementation
{
    public class PostService : IPostService
    {
        private readonly IPostImagesService _postImageService;
        private readonly IPostReactionsService _postReactionService;
        private readonly IPostCommentsService _postCommentService;
        private readonly IPostRepository _postRepository;
        
        public PostService(IPostImagesService postImageService, IPostReactionsService postReactionService,
            IPostCommentsService postCommentService, IPostRepository postRepository)
        {
            _postImageService = postImageService;
            _postReactionService = postReactionService;
            _postCommentService = postCommentService;
            _postRepository = postRepository;
        }
        public async Task<Post> CreatePostAsync(string userId, string textContent, List<IFormFile> images)
        {
            var post = new Post(textContent, userId);
            var createdPost = await _postRepository.CreateAsync(post);

            if(images?.Any() == true)
            {
                List<PostImages> postImages = new();
                foreach (var image in images)
                {
                    string path = FileManager.UploadFile("postImages", image);

                    postImages.Add(new PostImages(path, post.Id));
                }
                await _postImageService.AddRangeAsync(postImages);
            }
            return createdPost;
        }

        

    }
}
