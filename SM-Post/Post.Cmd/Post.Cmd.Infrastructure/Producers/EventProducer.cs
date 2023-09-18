using CQRS.Core.Producers;
using Messaging.Rabbitmq.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace Post.Cmd.Infrastructure.Producers
{
    public class EventProducer :IEventProducer
    {
        private readonly IServiceProvider _serviceProvider;

        public EventProducer(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }
        public Task ProduceAsync<T>(T message) where T:IQueueMessage  
        {
            var queueProducer = _serviceProvider.GetRequiredService<IQueueProducer<T>>();
            queueProducer.PublishMessage(message);
            return Task.CompletedTask;
        }
    }
}
