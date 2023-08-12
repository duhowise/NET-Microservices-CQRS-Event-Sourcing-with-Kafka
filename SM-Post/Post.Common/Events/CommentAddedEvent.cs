using CQRS.Core.Events;
using Mediator;

namespace Post.Common.Events;

public class CommentAddedEvent : BaseEvent, INotification
{
    public CommentAddedEvent() : base(nameof(CommentAddedEvent))
    {
    }

    public Guid CommentId { get; set; }
    public string Comment { get; set; } = null!;
    public string UserName { get; set; } = null!;
    public DateTime CommentDate { get; set; }
}