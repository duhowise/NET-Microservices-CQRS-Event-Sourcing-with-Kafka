using System;
using Confluent.Kafka;
using Microsoft.Extensions.Options;

namespace CQRS.Core.Consumers;

public class EventConsumer:IEventConsumer
{
    private readonly ConsumerConfig _config;

    public EventConsumer(IOptions<ConsumerConfig>config, IEventHandler eventHandler)
    {
        _config = config.Value;

    }
    public void Consume(string topic)
    {
        throw new NotImplementedException();
    }
}