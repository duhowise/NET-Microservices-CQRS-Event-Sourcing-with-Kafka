using Mediator;
using Post.Common.Events;

namespace Post.Query.Infrastructure.Handlers
{
    public interface IEventHandler : INotificationHandler<PostCreatedEvent>,
        INotificationHandler<MessageUpdatedEvent>,
        INotificationHandler<PostLikedEvent>, INotificationHandler<CommentAddedEvent>,
        INotificationHandler<CommentUpdatedEvent>
        , INotificationHandler<CommentRemovedEvent>, INotificationHandler<PostRemovedEvent>
    {
    }
}