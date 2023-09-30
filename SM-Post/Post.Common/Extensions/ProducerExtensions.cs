using Messaging.Rabbitmq.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Post.Common.Queue;

namespace Post.Common.Extensions;

public static class ProducerExtensions
{
    public static void AddQueueProducers(this IServiceCollection services)
    {
        services.AddScoped(typeof(IQueueProducer<>), typeof(QueueProducer<>));

    }

}