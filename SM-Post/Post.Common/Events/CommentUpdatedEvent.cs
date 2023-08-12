using CQRS.Core.Events;
using Mediator;

namespace Post.Common.Events;

public class CommentUpdatedEvent : BaseEvent, INotification
{
    public CommentUpdatedEvent() : base(nameof(CommentUpdatedEvent))
    {
    }

    public Guid CommentId { get; set; }
    public string Comment { get; set; } = null!;
    public string UserName { get; set; } = null!;
    public DateTime CommentDate { get; set; }
}