using Post.Common.Base;
using Post.Common.Events;

namespace Post.Query.Infrastructure.Consumers;

public class PostLikedQueueConsumer : QueueConsumer<PostLikedEvent>
{
    private readonly IEventHandler _eventHandler;

    public PostLikedQueueConsumer(IEventHandler eventHandler) : base(eventHandler)
    {
        _eventHandler = eventHandler;
    }
}  