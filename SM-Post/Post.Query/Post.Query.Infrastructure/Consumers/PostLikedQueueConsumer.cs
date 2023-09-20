using Messaging.Rabbitmq.Interfaces;
using Post.Common.Events;
using Post.Query.Infrastructure.Consumers.Base;
using Post.Query.Infrastructure.Handlers;

namespace Post.Query.Infrastructure.Consumers;

public class PostLikedQueueConsumer : QueueConsumer<PostLikedEvent>
{
    private readonly IEventHandler _eventHandler;

    public PostLikedQueueConsumer(IEventHandler eventHandler) : base(eventHandler)
    {
        _eventHandler = eventHandler;
    }
}  