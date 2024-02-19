using System.Diagnostics;
using OpenTelemetry.Trace;
using Post.Query.Domain.Entities;
using Post.Query.Domain.Repositories;

namespace Post.Query.Api.Queries
{
    public class QueryHandler : IQueryHandler
    {
        private readonly IPostRepository _postRepository;
        private readonly ActivitySource _activitySource = new ActivitySource("QueryHandler");
        public QueryHandler(IPostRepository postRepository)
        {
            _postRepository = postRepository;
        }

        public async Task<List<PostEntity>> HandleAsync(FindAllPostsQuery query)
        {
           

           using var activity = _activitySource.StartActivity("HandleAsync:FindAllPostsQuery");
            try
            {
                activity?.SetTag("query.type", query.GetType().Name);
                var result = await _postRepository.ListAllAsync();
                activity?.SetTag("result.count", result.Count);
                return result;
            }
            catch(Exception ex)
            {
                activity?.SetTag("error", true);
                activity?.SetTag("error.message", ex.Message);
                activity?.SetStatus(Status.Error.WithDescription(ex.Message));
                throw;
            }
        }

        public async Task<List<PostEntity>> HandleAsync(FindPostByIdQuery query)
        {
            var post = await _postRepository.GetByIdAsync(query.Id);
            return new List<PostEntity> { post };
        }

        public async Task<List<PostEntity>> HandleAsync(FindPostsByAuthorQuery query)
        {
            return await _postRepository.ListByAuthorAsync(query.Author);
        }

        public async Task<List<PostEntity>> HandleAsync(FindPostsWithCommentsQuery query)
        {
            return await _postRepository.ListWithCommentsAsync();
        }

        public async Task<List<PostEntity>> HandleAsync(FindPostsWithLikesQuery query)
        {
            return await _postRepository.ListWithLikesAsync(query.NumberOfLikes);
        }
    }
}