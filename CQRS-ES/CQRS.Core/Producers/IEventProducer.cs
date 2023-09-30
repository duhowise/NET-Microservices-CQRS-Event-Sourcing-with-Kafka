using System.Threading.Tasks;
using Messaging.Rabbitmq.Interfaces;

namespace CQRS.Core.Producers;

public interface IEventProducer 
{
    Task ProduceAsync<T>(T @event)where T:IQueueMessage;
}