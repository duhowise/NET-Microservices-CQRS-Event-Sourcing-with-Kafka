using Post.Common.Base;
using Post.Common.Events;

namespace Post.Query.Infrastructure.Consumers;

public class CommentAddedQueueConsumer : QueueConsumer<CommentAddedEvent>
{
    private readonly IEventHandler _eventHandler;

    public CommentAddedQueueConsumer(IEventHandler eventHandler) : base(eventHandler)
    {
        _eventHandler = eventHandler;
    }
}