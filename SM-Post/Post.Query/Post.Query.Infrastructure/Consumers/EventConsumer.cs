using System;
using System.Text.Json;
using Confluent.Kafka;
using CQRS.Core.Consumers;
using CQRS.Core.Events;
using Microsoft.Extensions.Options;
using Post.Query.Infrastructure.Converter;
using Post.Query.Infrastructure.Handlers;

namespace Post.Query.Infrastructure.Consumers;

public class EventConsumer : IEventConsumer
{
    private readonly IEventHandler _eventHandler;
    private readonly ConsumerConfig _config;

    public EventConsumer(IOptions<ConsumerConfig> config, IEventHandler eventHandler)
    {
        _eventHandler = eventHandler;
        _config = config.Value;
    }
    public void Consume(string topic)
    {
        using var consumer = new ConsumerBuilder<string, string>(_config)
            .SetKeyDeserializer(Deserializers.Utf8)
            .SetValueDeserializer(Deserializers.Utf8)
            .Build();
        consumer.Subscribe(topic);
        while (true)
        {
            var consumerResult= consumer.Consume();
            if(consumerResult?.Message==null) continue;
            var options = new JsonSerializerOptions { Converters = { new EventJsonConverter() } };
            var @event = JsonSerializer.Deserialize<BaseEvent>(consumerResult.Value, options);
            var handleMethod = _eventHandler.GetType().GetMethod("On", new Type[] { @event.GetType() });
            if (handleMethod==null)
            {
                throw new ArgumentNullException(nameof(handleMethod), "could not find event handler method!");
            }

            handleMethod.Invoke(_eventHandler, new object[] { @event });
            consumer.Commit(consumerResult);

        }
    }
}