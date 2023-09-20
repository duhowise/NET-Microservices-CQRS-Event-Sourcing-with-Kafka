﻿using System.Text.Json;
using Confluent.Kafka;
using CQRS.Core.Events;
using Messaging.Rabbitmq.Interfaces;
using Microsoft.Extensions.Options;
using Post.Common.Converter;
using Post.Query.Infrastructure.Handlers;

namespace Post.Query.Infrastructure.Consumers;

public class EventConsumer<TEvent> : IQueueConsumer<TEvent> where TEvent : BaseEvent
{
    private readonly IEventHandler _eventHandler;
    private readonly ConsumerConfig _config;

    public EventConsumer(IOptions<ConsumerConfig> config, IEventHandler eventHandler)
    {
        _eventHandler = eventHandler;
        _config = config.Value;
    }

    public Task ConsumeAsync(TEvent message)
    {
        var options = new JsonSerializerOptions { Converters = { new EventJsonConverter() } };
        var handleMethod = _eventHandler.GetType().GetMethod("On", new Type[] { message.GetType() });
        if (handleMethod==null)
        {
            throw new ArgumentNullException(nameof(handleMethod), "could not find event handler method!");
        }

        handleMethod.Invoke(_eventHandler, new object[] { message });
        return Task.CompletedTask;
    }
}