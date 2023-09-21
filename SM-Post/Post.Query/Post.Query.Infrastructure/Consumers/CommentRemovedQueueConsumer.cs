using Post.Common.Base;
using Post.Common.Events;

namespace Post.Query.Infrastructure.Consumers;

public class CommentRemovedQueueConsumer : QueueConsumer<CommentRemovedEvent>
{
    private readonly IEventHandler _eventHandler;

    public CommentRemovedQueueConsumer(IEventHandler eventHandler) : base(eventHandler)
    {
        _eventHandler = eventHandler;
    }
}