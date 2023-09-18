﻿namespace Messaging.Rabbitmq.Interfaces
{
    public interface IQueueProducer<in TQueueMessage> where TQueueMessage : IQueueMessage
    {
        void PublishMessage(TQueueMessage message);
    }
}