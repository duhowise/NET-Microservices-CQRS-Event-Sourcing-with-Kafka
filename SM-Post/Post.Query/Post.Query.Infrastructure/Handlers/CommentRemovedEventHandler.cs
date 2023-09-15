using Post.Common.Events;
using Post.Query.Domain.Repositories;

namespace Post.Query.Infrastructure.Handlers;

public class CommentRemovedEventHandler : IEventHandler<CommentRemovedEvent>
{
    private readonly ICommentRepository _commentRepository;

    public CommentRemovedEventHandler(ICommentRepository commentRepository)
    {
        _commentRepository = commentRepository;
    }


    public async Task On(CommentRemovedEvent @event)
    {
        await _commentRepository.DeleteAsync(@event.Id);
    }
}