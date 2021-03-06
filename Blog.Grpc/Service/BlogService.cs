using Grpc.Core;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using Google.Protobuf.WellKnownTypes;

namespace Blog.Grpc
{
    public class BlogService : BlogServiceInternal.BlogServiceInternalBase
    {

        private readonly ILogger<BlogService> logger;

        public BlogService(ILogger<BlogService> logger)
        {
            this.logger = logger;
        }

        public override Task<CreatePostResponse> CreatePost(CreatePostRequest request, ServerCallContext context)
        {
            logger.LogInformation($"Create post \"{request.Title}\" by {request.Author}");

            using var postGateway = new PostGateway();

            var response = new CreatePostResponse
            {
                Success = postGateway.Insert(request.Author, request.Title, request.Content)
            };

            return Task.FromResult(response);
        }

        public override Task<GetPostResponse> GetPost(GetPostRequest request, ServerCallContext context)
        {
            logger.LogInformation($"Get post {request.Id}");

            using var postGateway = new PostGateway();
            var post = postGateway.Select(request.Id);


            var response = post switch
            {
                null => new GetPostResponse { Success = false, ErrorMessage = $"Post id = {request.Id} not found" },
                not null => new GetPostResponse
                {
                    Author = post.Author,
                    Title = post.Title,
                    Content = post.Content,
                    Date = Timestamp.FromDateTime(post.DatePosted),
                    Success = true,
                }
            };

            return Task.FromResult(response);
        }

        public override Task<UpdatePostResponse> UpdatePost(UpdatePostRequest request, ServerCallContext context)
        {
            logger.LogInformation($"Update post {request.Id}");

            using var postGateway = new PostGateway();

            var response = new UpdatePostResponse
            {
                Success = postGateway.Update(request.Id, request.Author, request.Title, request.Content)
            };

            return Task.FromResult(response);
        }

        public override Task<DeletePostResponse> DeletePost(DeletePostRequest request, ServerCallContext context)
        {
            logger.LogInformation($"Delete post {request.Id}");

            using var postGateway = new PostGateway();

            var response = new DeletePostResponse
            {
                Success = postGateway.Delete(request.Id)
            };

            return Task.FromResult(new DeletePostResponse());
        }
    }
}
