using Post.Common.Events;
using Post.Query.Domain.Entities;
using Post.Query.Domain.Repositories;

namespace Post.Query.Infrastructure.Handlers;

public class PostCreatedEventHandler : IEventHandler<PostCreatedEvent>
{
    private readonly IPostRepository _postRepository;

    public PostCreatedEventHandler(IPostRepository postRepository)
    {
        _postRepository = postRepository;
    }

    public async Task On(PostCreatedEvent @event)
    {
        var post = new PostEntity
        {
            PostId = @event.Id,
            Author = @event.Author,
            DatePosted = @event.DatePosted,
            Message = @event.Message,
        };
        await _postRepository.CreateAsync(post);
    }
}