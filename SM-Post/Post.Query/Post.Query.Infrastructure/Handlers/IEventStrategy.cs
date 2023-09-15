namespace Post.Query.Infrastructure.Handlers
{

    public interface IEventStrategy
    {
        IEventHandler<TEvent> GetHandler<TEvent>(TEvent @event);
    }
}