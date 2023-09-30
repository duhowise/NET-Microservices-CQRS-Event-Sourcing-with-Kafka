using RabbitMQ.Client;

namespace Messaging.Rabbitmq.Interfaces
{
    public interface IConnectionProvider
    {
        IConnection GetConnection();
    }
}