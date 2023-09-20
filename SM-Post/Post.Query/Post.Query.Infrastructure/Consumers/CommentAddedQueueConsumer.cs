using Messaging.Rabbitmq.Interfaces;
using Post.Common.Events;
using Post.Query.Infrastructure.Consumers.Base;
using Post.Query.Infrastructure.Handlers;

namespace Post.Query.Infrastructure.Consumers;

public class CommentAddedQueueConsumer : QueueConsumer<CommentAddedEvent>
{
    private readonly IEventHandler _eventHandler;

    public CommentAddedQueueConsumer(IEventHandler eventHandler) : base(eventHandler)
    {
        _eventHandler = eventHandler;
    }
}