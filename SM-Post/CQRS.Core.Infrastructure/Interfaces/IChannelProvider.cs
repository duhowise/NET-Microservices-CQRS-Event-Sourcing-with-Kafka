using RabbitMQ.Client;

namespace Messaging.Rabbitmq.Interfaces
{
    public interface IChannelProvider
    {
        IModel GetChannel();
    }
}