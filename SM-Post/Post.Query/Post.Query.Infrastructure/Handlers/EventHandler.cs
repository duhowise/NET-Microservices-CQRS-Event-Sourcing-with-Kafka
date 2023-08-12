using Post.Common.Events;
using Post.Query.Domain.Entities;
using Post.Query.Domain.Repositories;

namespace Post.Query.Infrastructure.Handlers;

public class EventHandler:IEventHandler
{
    private readonly IPostRepository _postRepository;
    private readonly ICommentRepository _commentRepository;

    public EventHandler(IPostRepository postRepository,ICommentRepository commentRepository)
    {
        _postRepository = postRepository;
        _commentRepository = commentRepository;
    }

    public async ValueTask Handle(PostCreatedEvent command, CancellationToken cancellationToken)
    {
       
            var post = new PostEntity
            {
                PostId = command.Id,
                Author = command.Author,
                DatePosted = command.DatePosted,
                Message = command.Message,
            };
             await _postRepository.CreateAsync(post);
    }

    public async ValueTask Handle(MessageUpdatedEvent command, CancellationToken cancellationToken)
    {
        var post =await _postRepository.GetByIdAsync(command.Id);
        if (post == null) return ;
        post.Message = command.Message;
        await _postRepository.UpdateAsync(post);
    }

    public async ValueTask Handle(PostLikedEvent command, CancellationToken cancellationToken)
    {
        var post = await _postRepository.GetByIdAsync(command.Id);
        if (post == null) return;
        post.Likes++;
        await _postRepository.UpdateAsync(post);
    }

    public async ValueTask Handle(CommentAddedEvent command, CancellationToken cancellationToken)
    {
        var comment = new CommentEntity
        {
            PostId = command.Id,
            Comment = command.Comment,
            CommentDate = command.CommentDate,
            CommentId = command.CommentId,
            Edited = false,
            UserName = command.UserName
        };

        await _commentRepository.CreateAsync(comment);
    }

    public async ValueTask Handle(CommentUpdatedEvent command, CancellationToken cancellationToken)
    {
        var comment=await _commentRepository.GetByIdAsync(command.Id);
        if (comment == null) return;

        comment.CommentDate = command.CommentDate;
        comment.Edited = true;
        await _commentRepository.UpdateAsync(comment);
    }

    public async ValueTask Handle(CommentRemovedEvent command, CancellationToken cancellationToken)
    {
        await _commentRepository.DeleteAsync(command.Id);
    }

    public async ValueTask Handle(PostRemovedEvent command, CancellationToken cancellationToken)
    {
        await _postRepository.DeleteAsync(command.Id);
    }
}