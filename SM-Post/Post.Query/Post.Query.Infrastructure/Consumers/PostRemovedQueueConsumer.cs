using Messaging.Rabbitmq.Interfaces;
using Post.Common.Base;
using Post.Common.Events;
using Post.Query.Infrastructure.Handlers;

namespace Post.Query.Infrastructure.Consumers;

public class PostRemovedQueueConsumer : QueueConsumer<PostRemovedEvent>
{
    private readonly IEventHandler _eventHandler;

    public PostRemovedQueueConsumer(IEventHandler eventHandler) : base(eventHandler)
    {
        _eventHandler = eventHandler;
    }
}