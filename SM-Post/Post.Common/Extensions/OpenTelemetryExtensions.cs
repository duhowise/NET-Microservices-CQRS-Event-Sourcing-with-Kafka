using Microsoft.Extensions.DependencyInjection;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Post.Common.Options;

namespace Post.Common.Extensions;

public static class OpenTelemetryExtensions
{
    public static IServiceCollection AddOpenTelemetryWithJaeger(this IServiceCollection services, string serviceName,
        OpenTelemetryConfig jaegerConfig)
    {
        services.AddOpenTelemetry()
            .ConfigureResource(resourceBuilder => { resourceBuilder.AddService(serviceName); })
            .WithTracing(builder =>
            {
                builder.AddAspNetCoreInstrumentation()
                    .AddHttpClientInstrumentation()
                    .AddSource("QueueProducer")
                    .AddSource("EventSourcingHandler")
                    .AddSource("EventHandler")
                    .AddSource("QueryHandler")
                    .AddSource("QueueConsumer")
                    .AddSource("CommandHandler")
                    .SetResourceBuilder(ResourceBuilder.CreateDefault().AddService(serviceName))
#if DEBUG
                    .AddConsoleExporter()       
#endif
                    .AddOtlpExporter(options =>
                    {
                      options.Endpoint=  new Uri($"{jaegerConfig.Protocol}://{jaegerConfig.Host}:{jaegerConfig.Port}");
                    });
            })
            ;
        return services;
    }
}