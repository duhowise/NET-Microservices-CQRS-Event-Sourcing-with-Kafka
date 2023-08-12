using CQRS.Core.Handlers;
using Mediator;
using Post.Cmd.Domain.Aggregates;

namespace Post.Cmd.Api.Commands;

public class CommandHandler : ICommandHandler
{
    private readonly IEventSourcingHandler<PostAggregate> _eventSourcingHandler;

    public CommandHandler(IEventSourcingHandler<PostAggregate> eventSourcingHandler)
    {
        _eventSourcingHandler = eventSourcingHandler;
    }
    public async ValueTask<Unit> Handle(NewPostCommand command, CancellationToken cancellationToken)
    {
        var aggregate=new PostAggregate(command.Id,command.Author,command.Message);
        await _eventSourcingHandler.SaveAsync(aggregate);
        return Unit.Value;
    }

    public async ValueTask<Unit> Handle(EditMessageCommand command, CancellationToken cancellationToken)
    {
        var aggregate=  await _eventSourcingHandler.GetByIdAsync(command.Id);
        aggregate.EditMessage(command.Message);
        await _eventSourcingHandler.SaveAsync(aggregate);
        return Unit.Value;
    }

    public async ValueTask<Unit> Handle(LikePostCommand command, CancellationToken cancellationToken)
    {
        var aggregate = await _eventSourcingHandler.GetByIdAsync(command.Id);
        aggregate.LikePost();
        await _eventSourcingHandler.SaveAsync(aggregate);
        return Unit.Value;
    }

    public async ValueTask<Unit> Handle(AddCommentCommand command, CancellationToken cancellationToken)
    {
        var aggregate = await _eventSourcingHandler.GetByIdAsync(command.Id);
        aggregate.AddComment(command.Comment,command.UserName);
        await _eventSourcingHandler.SaveAsync(aggregate);
        return Unit.Value;
    }

    public async ValueTask<Unit> Handle(EditCommentCommand command, CancellationToken cancellationToken)
    {
        var aggregate = await _eventSourcingHandler.GetByIdAsync(command.Id);
        aggregate.EditComment(command.CommentId,command.Comment,command.UserName);
        await _eventSourcingHandler.SaveAsync(aggregate);
        return Unit.Value;
    }

    public async ValueTask<Unit> Handle(RemoveCommentCommand command, CancellationToken cancellationToken)
    {
        var aggregate = await _eventSourcingHandler.GetByIdAsync(command.Id);
        aggregate.RemoveComment(command.CommentId,command.UserName);
        await _eventSourcingHandler.SaveAsync(aggregate);
        return Unit.Value;
    }

    public async ValueTask<Unit> Handle(DeletePostCommand command, CancellationToken cancellationToken)
    {
        var aggregate = await _eventSourcingHandler.GetByIdAsync(command.Id);
        aggregate.DeletePost(command.UserName);
        await _eventSourcingHandler.SaveAsync(aggregate);
        return Unit.Value;
    }
}