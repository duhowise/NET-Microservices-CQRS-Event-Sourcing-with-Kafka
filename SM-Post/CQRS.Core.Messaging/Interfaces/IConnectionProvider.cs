using RabbitMQ.Client;

namespace Messaging.Rabbitmq.Interfaces
{
    internal interface IConnectionProvider
    {
        IConnection GetConnection();
    }
}