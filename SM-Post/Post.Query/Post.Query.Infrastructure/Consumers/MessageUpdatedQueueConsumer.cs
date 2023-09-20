using Messaging.Rabbitmq.Interfaces;
using Post.Common.Base;
using Post.Common.Events;
using Post.Query.Infrastructure.Handlers;

namespace Post.Query.Infrastructure.Consumers;

public class MessageUpdatedQueueConsumer : QueueConsumer<MessageUpdatedEvent>
{
    private readonly IEventHandler _eventHandler;

    public MessageUpdatedQueueConsumer(IEventHandler eventHandler) : base(eventHandler)
    {
        _eventHandler = eventHandler;
    }
}