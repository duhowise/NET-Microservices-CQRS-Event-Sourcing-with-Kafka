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
                    .AddSource(serviceName)
                    .SetResourceBuilder(ResourceBuilder.CreateDefault().AddService(serviceName))
                    .AddOtlpExporter(options =>
                    {
                      options.Endpoint=  new Uri($"{jaegerConfig.Protocol}://{jaegerConfig.Host}:{jaegerConfig.Port}");
                    });
            })
            ;
        return services;
    }
}