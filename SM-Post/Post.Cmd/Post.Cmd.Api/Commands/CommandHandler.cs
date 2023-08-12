using CQRS.Core.Handlers;
using Mediator;
using Post.Cmd.Domain.Aggregates;

namespace Post.Cmd.Api.Commands;

public class CommandHandler : ICommandHandler
{
    private readonly IServiceScopeFactory _scopeFactory;

    public CommandHandler(IServiceScopeFactory scopeFactory)
    {
        _scopeFactory = scopeFactory;
    }
    public async ValueTask<Unit> Handle(NewPostCommand command, CancellationToken cancellationToken)
    {
        using var serviceScope = _scopeFactory.CreateScope();
        var eventSourcingHandler = serviceScope.ServiceProvider.GetService<IEventSourcingHandler<PostAggregate>>();
        var aggregate=new PostAggregate(command.Id,command.Author,command.Message);
        await eventSourcingHandler.SaveAsync(aggregate);
        return Unit.Value;
    }

    public async ValueTask<Unit> Handle(EditMessageCommand command, CancellationToken cancellationToken)
    {
        using var serviceScope = _scopeFactory.CreateScope();
        var eventSourcingHandler = serviceScope.ServiceProvider.GetService<IEventSourcingHandler<PostAggregate>>();
        var aggregate=  await eventSourcingHandler.GetByIdAsync(command.Id);
        aggregate.EditMessage(command.Message);
        await eventSourcingHandler.SaveAsync(aggregate);
        return Unit.Value;
    }

    public async ValueTask<Unit> Handle(LikePostCommand command, CancellationToken cancellationToken)
    {
        using var serviceScope = _scopeFactory.CreateScope();
        var eventSourcingHandler = serviceScope.ServiceProvider.GetService<IEventSourcingHandler<PostAggregate>>();
        var aggregate = await eventSourcingHandler.GetByIdAsync(command.Id);
        aggregate.LikePost();
        await eventSourcingHandler.SaveAsync(aggregate);
        return Unit.Value;
    }

    public async ValueTask<Unit> Handle(AddCommentCommand command, CancellationToken cancellationToken)
    {
        using var serviceScope = _scopeFactory.CreateScope();
        var eventSourcingHandler = serviceScope.ServiceProvider.GetService<IEventSourcingHandler<PostAggregate>>();
        var aggregate = await eventSourcingHandler.GetByIdAsync(command.Id);
        aggregate.AddComment(command.Comment,command.UserName);
        await eventSourcingHandler.SaveAsync(aggregate);
        return Unit.Value;
    }

    public async ValueTask<Unit> Handle(EditCommentCommand command, CancellationToken cancellationToken)
    {
        using var serviceScope = _scopeFactory.CreateScope();
        var eventSourcingHandler = serviceScope.ServiceProvider.GetService<IEventSourcingHandler<PostAggregate>>();
        var aggregate = await eventSourcingHandler.GetByIdAsync(command.Id);
        aggregate.EditComment(command.CommentId,command.Comment,command.UserName);
        await eventSourcingHandler.SaveAsync(aggregate);
        return Unit.Value;
    }

    public async ValueTask<Unit> Handle(RemoveCommentCommand command, CancellationToken cancellationToken)
    {
        using var serviceScope = _scopeFactory.CreateScope();
        var eventSourcingHandler = serviceScope.ServiceProvider.GetService<IEventSourcingHandler<PostAggregate>>();
        var aggregate = await eventSourcingHandler.GetByIdAsync(command.Id);
        aggregate.RemoveComment(command.CommentId,command.UserName);
        await eventSourcingHandler.SaveAsync(aggregate);
        return Unit.Value;
    }

    public async ValueTask<Unit> Handle(DeletePostCommand command, CancellationToken cancellationToken)
    {
        using var serviceScope = _scopeFactory.CreateScope();
        var eventSourcingHandler = serviceScope.ServiceProvider.GetService<IEventSourcingHandler<PostAggregate>>();
        var aggregate = await eventSourcingHandler.GetByIdAsync(command.Id);
        aggregate.DeletePost(command.UserName);
        await eventSourcingHandler.SaveAsync(aggregate);
        return Unit.Value;
    }
}