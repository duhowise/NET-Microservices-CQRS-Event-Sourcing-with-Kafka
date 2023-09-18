using System.Threading.Tasks;

namespace Messaging.Rabbitmq.Interfaces
{
    public interface IQueueConsumer<in TQueueMessage> where TQueueMessage : class, IQueueMessage
    {
        Task ConsumeAsync(TQueueMessage message);
    }
}