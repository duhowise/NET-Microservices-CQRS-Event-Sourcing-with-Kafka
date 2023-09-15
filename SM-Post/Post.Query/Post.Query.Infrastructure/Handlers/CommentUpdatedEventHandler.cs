using Post.Common.Events;
using Post.Query.Domain.Entities;
using Post.Query.Domain.Repositories;

namespace Post.Query.Infrastructure.Handlers;

public class CommentUpdatedEventHandler : IEventHandler<CommentUpdatedEvent>
{
    private readonly ICommentRepository _commentRepository;

    public CommentUpdatedEventHandler(ICommentRepository commentRepository)
    {
        _commentRepository = commentRepository;
    }


    public async Task On(CommentUpdatedEvent @event)
    {
        var comment=await _commentRepository.GetByIdAsync(@event.Id);
        if (comment == null) return;

        comment.CommentDate = @event.CommentDate;
        comment.Edited = true;
        await _commentRepository.UpdateAsync(comment);
    }
}