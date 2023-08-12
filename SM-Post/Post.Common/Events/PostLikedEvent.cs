using CQRS.Core.Events;
using Mediator;

namespace Post.Common.Events;

public class PostLikedEvent : BaseEvent, INotification
{
    public PostLikedEvent() : base(nameof(PostLikedEvent))
    {
    }
}