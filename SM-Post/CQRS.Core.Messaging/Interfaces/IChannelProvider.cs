using RabbitMQ.Client;

namespace Messaging.Rabbitmq.Interfaces
{
    internal interface IChannelProvider
    {
        IModel GetChannel();
    }
}