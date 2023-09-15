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
    private readonly IEventStrategy _eventStrategy;
    private readonly ConsumerConfig _config;

    public EventConsumer(IOptions<ConsumerConfig> config, IEventStrategy eventStrategy)
    {
        _eventStrategy = eventStrategy;
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
            var consumerResult = consumer.Consume();
            if (consumerResult?.Message == null) continue;
            var options = new JsonSerializerOptions { Converters = { new EventJsonConverter() } };
            var baseEvent = JsonSerializer.Deserialize<BaseEvent>(consumerResult.Message.Value, options);
            var handler = _eventStrategy.GetHandler(baseEvent);
            if (handler == null)
            {
                throw new ArgumentNullException(nameof(handler), "could not find event handler method!");
            }
            handler.On(baseEvent);
            consumer.Commit(consumerResult);
        }
    }
}