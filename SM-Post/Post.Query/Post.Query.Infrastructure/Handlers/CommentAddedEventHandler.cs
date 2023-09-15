using Post.Common.Events;
using Post.Query.Domain.Entities;
using Post.Query.Domain.Repositories;

namespace Post.Query.Infrastructure.Handlers;

public class CommentAddedEventHandler : IEventHandler<CommentAddedEvent>
{
    private readonly ICommentRepository _commentRepository;

    public CommentAddedEventHandler(ICommentRepository commentRepository)
    {
        _commentRepository = commentRepository;
    }


    public async Task On(CommentAddedEvent @event)
    {
        var comment = new CommentEntity
        {
            PostId = @event.Id,
            Comment = @event.Comment,
            CommentDate = @event.CommentDate,
            CommentId = @event.CommentId,
            Edited = false,
            UserName = @event.UserName
        };

        await _commentRepository.CreateAsync(comment);
    }
}