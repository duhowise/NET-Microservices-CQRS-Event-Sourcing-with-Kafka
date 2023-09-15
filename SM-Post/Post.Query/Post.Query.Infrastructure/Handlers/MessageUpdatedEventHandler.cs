using Post.Common.Events;
using Post.Query.Domain.Repositories;

namespace Post.Query.Infrastructure.Handlers;

public class MessageUpdatedEventHandler : IEventHandler<MessageUpdatedEvent>
{
    private readonly IPostRepository _postRepository;

    public MessageUpdatedEventHandler(IPostRepository postRepository)
    {
        _postRepository = postRepository;
    }

    public async Task On(MessageUpdatedEvent @event)
    {
        var post =await _postRepository.GetByIdAsync(@event.Id);
        if (post == null) return;
        post.Message = @event.Message;
        await _postRepository.UpdateAsync(post);
    }
}