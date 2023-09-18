using System;
using Messaging.Rabbitmq.Interfaces;

namespace CQRS.Core.Messages;

public abstract class Message : IQueueMessage
{
    public Guid Id { get; set; }
    public TimeSpan TimeToLive { get; set; } = TimeSpan.FromMilliseconds(5 * 60 * 1000);
}