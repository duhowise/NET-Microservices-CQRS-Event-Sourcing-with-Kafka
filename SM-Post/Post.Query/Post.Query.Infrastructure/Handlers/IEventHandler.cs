using Mediator;
using Post.Common.Events;

namespace Post.Query.Infrastructure.Handlers
{
    public interface IEventHandler : ICommandHandler<PostCreatedEvent>,
        ICommandHandler<MessageUpdatedEvent>,
        ICommandHandler<PostLikedEvent>, ICommandHandler<CommentAddedEvent>, ICommandHandler<CommentUpdatedEvent>
        , ICommandHandler<CommentRemovedEvent>, ICommandHandler<PostRemovedEvent>
    {
    }
}