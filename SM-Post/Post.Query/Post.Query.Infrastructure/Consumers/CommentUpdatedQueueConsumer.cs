using Messaging.Rabbitmq.Interfaces;
using Post.Common.Base;
using Post.Common.Events;
using Post.Query.Infrastructure.Handlers;

namespace Post.Query.Infrastructure.Consumers;

public class CommentUpdatedQueueConsumer : QueueConsumer<CommentUpdatedEvent>
{
    private readonly IEventHandler _eventHandler;

    public CommentUpdatedQueueConsumer(IEventHandler eventHandler) : base(eventHandler)
    {
        _eventHandler = eventHandler;
    }
}