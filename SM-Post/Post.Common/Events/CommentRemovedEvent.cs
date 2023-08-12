using CQRS.Core.Events;
using Mediator;

namespace Post.Common.Events;

public class CommentRemovedEvent : BaseEvent, ICommand
{
    public CommentRemovedEvent() : base(nameof(CommentRemovedEvent))
    {
    }

    public Guid CommentId { get; set; }
}