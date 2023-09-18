using System;

namespace Messaging.Rabbitmq.Interfaces
{
    public interface IQueueMessage
    {
        public Guid Id { get; set; }
        TimeSpan TimeToLive { get; set; }
    }
}