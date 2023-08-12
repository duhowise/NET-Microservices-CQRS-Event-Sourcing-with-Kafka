using Microsoft.Extensions.DependencyInjection;

namespace Post.Query.Infrastructure.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection RegisterInfrastructureServices(this IServiceCollection services)
    {
        services.AddMediator();
        return services;
    }
}