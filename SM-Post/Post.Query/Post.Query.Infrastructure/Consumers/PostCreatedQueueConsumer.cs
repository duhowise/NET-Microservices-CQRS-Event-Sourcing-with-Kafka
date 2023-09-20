using System.Text.Json;
using CQRS.Core.Events;
using Messaging.Rabbitmq.Interfaces;
using Post.Common.Base;
using Post.Common.Converter;
using Post.Common.Events;
using Post.Query.Infrastructure.Handlers;

namespace Post.Query.Infrastructure.Consumers;

public class PostCreatedQueueConsumer : QueueConsumer<PostCreatedEvent>
{
    private readonly IEventHandler _eventHandler;

    public PostCreatedQueueConsumer(IEventHandler eventHandler) : base(eventHandler)
    {
        _eventHandler = eventHandler;
    }
}