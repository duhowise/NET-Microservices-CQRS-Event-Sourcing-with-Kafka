using System.Text.Json;
using CQRS.Core.Events;
using Messaging.Rabbitmq.Interfaces;
using Post.Common.Converter;
using Post.Common.Events;
using Post.Query.Infrastructure.Handlers;

namespace Post.Query.Infrastructure.Consumers;

public class PostCreatedEventConsumer : IQueueConsumer<PostCreatedEvent>
{
    private readonly IEventHandler _eventHandler;

    public PostCreatedEventConsumer(IEventHandler eventHandler)
    {
        _eventHandler = eventHandler;
    }

    public Task ConsumeAsync(PostCreatedEvent message)
    {
        var handleMethod = _eventHandler.GetType().GetMethod("On", new Type[] { message.GetType() });
        if (handleMethod==null)
        {
            throw new ArgumentNullException(nameof(handleMethod), "could not find event handler method!");
        }

        handleMethod.Invoke(_eventHandler, new object[] { message });
        return Task.CompletedTask;
    }
}