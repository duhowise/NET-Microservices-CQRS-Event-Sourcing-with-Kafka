using Microsoft.Extensions.DependencyInjection;
using Post.Common.Events;

namespace Post.Query.Infrastructure.Handlers;

public class EventStrategy : IEventStrategy
{
    private readonly IServiceScopeFactory _serviceScopeFactory;

    public EventStrategy(IServiceScopeFactory serviceScopeFactory)
    {
        _serviceScopeFactory = serviceScopeFactory;
    }


    public IEventHandler<TEvent> GetHandler<TEvent>(TEvent @event)
    {
        using var scope = _serviceScopeFactory.CreateScope();
        {
            foreach (var service in scope.ServiceProvider.GetServices<IEnumerable<object>>())
            {
                Console.WriteLine(service.GetType().Name);
            }
            var handler = scope.ServiceProvider.GetService<IEventHandler<TEvent>>();
            return handler;
        }
        
    }
}