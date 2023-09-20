using Messaging.Rabbitmq.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Post.Common.Base;

namespace Post.Query.Infrastructure.Extensions;

public static class ConsumerExtensions
{
    public static void AddQueueConsumers(this IServiceCollection services)
    {
        services.AddScoped(typeof(IQueueConsumer<>), typeof(QueueConsumer<>));

    }

}