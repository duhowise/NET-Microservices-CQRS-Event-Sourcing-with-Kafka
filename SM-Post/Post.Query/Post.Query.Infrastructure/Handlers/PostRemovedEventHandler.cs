using Post.Common.Events;
using Post.Query.Domain.Repositories;

namespace Post.Query.Infrastructure.Handlers;

public class PostRemovedEventHandler : IEventHandler<PostRemovedEvent>
{
    private readonly IPostRepository _postRepository;

    public PostRemovedEventHandler(IPostRepository postRepository)
    {
        _postRepository = postRepository;
    }

    public async Task On(PostRemovedEvent @event)
    {
        await _postRepository.DeleteAsync(@event.Id);
    }
}