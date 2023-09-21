using Post.Common.Base;
using Post.Common.Events;

namespace Post.Query.Infrastructure.Consumers;

public class PostCreatedQueueConsumer : QueueConsumer<PostCreatedEvent>
{
    private readonly IEventHandler _eventHandler;

    public PostCreatedQueueConsumer(IEventHandler eventHandler) : base(eventHandler)
    {
        _eventHandler = eventHandler;
    }
}