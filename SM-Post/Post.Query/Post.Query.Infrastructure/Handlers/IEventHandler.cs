using CQRS.Core.Events;

namespace Post.Query.Infrastructure.Handlers
{
    public interface IEventHandler<in TEvent>
    {
        Task On(TEvent @event);
    }
}
