using CQRS.Core.Events;
using Mediator;

namespace Post.Common.Events;

public class PostRemovedEvent : BaseEvent, ICommand
{
    public PostRemovedEvent() : base(nameof(PostRemovedEvent))
    {
    }
}