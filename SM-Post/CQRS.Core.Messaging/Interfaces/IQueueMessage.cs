using System;

namespace Messaging.Rabbitmq.Interfaces
{
    public interface IQueueMessage
    {
        Guid MessageId { get; set; }
        TimeSpan TimeToLive { get; set; }
    }
}