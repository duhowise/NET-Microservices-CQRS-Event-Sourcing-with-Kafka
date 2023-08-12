using CQRS.Core.Events;
using Mediator;

namespace Post.Common.Events;

public class PostCreatedEvent : BaseEvent,INotification
{
    public PostCreatedEvent() : base(nameof(PostCreatedEvent))
    {
    }

    public string Author { get; set; } = null!;
    public string Message { get; set; } = null!;
    public DateTime DatePosted { get; set; }
}