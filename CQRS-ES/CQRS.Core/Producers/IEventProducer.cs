using System.Threading.Tasks;
using CQRS.Core.Events;
using Messaging.Rabbitmq.Interfaces;

namespace CQRS.Core.Producers;

public interface IEventProducer 
{
    Task ProduceAsync<T>(T @event)where T:IQueueMessage;
}