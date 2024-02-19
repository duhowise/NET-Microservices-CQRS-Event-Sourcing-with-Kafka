using System.Diagnostics;
using CQRS.Core.Handlers;
using Microsoft.Extensions.Options;
using OpenTelemetry.Trace;
using Post.Cmd.Domain.Aggregates;

namespace Post.Cmd.Api.Commands;

public class CommandHandler : ICommandHandler
{
    private readonly IEventSourcingHandler<PostAggregate> _eventSourcingHandler;
    private readonly ActivitySource _activitySource = new ActivitySource("CommandHandler");

    public CommandHandler(IEventSourcingHandler<PostAggregate> eventSourcingHandler)
    {
        _eventSourcingHandler = eventSourcingHandler;
    }

    public async Task HandleAsync(NewPostCommand command)
    {
        using var activity = _activitySource.StartActivity($"Handling_{nameof(command)}", ActivityKind.Consumer);

        activity?.SetTag("Command.Author", command.Author);
        activity?.SetTag("Command.Message", command.Message);

        try
        {
            var aggregate = new PostAggregate(command.Id, command.Author, command.Message);
            await _eventSourcingHandler.SaveAsync(aggregate);
        }
        catch (Exception ex)
        {
            activity?.SetStatus(ActivityStatusCode.Error, $"An exception occurred: {ex.Message}");

            throw;
        }
    }

    public async Task HandleAsync(EditMessageCommand command)
    {
        using var activity = _activitySource.StartActivity($"Handling_{nameof(command)}", ActivityKind.Consumer);

        try
        {
            var aggregate = await _eventSourcingHandler.GetByIdAsync(command.Id);
            aggregate.EditMessage(command.Message);
            await _eventSourcingHandler.SaveAsync(aggregate);
            
            activity?.SetTag("status", "success");
        }
        catch (Exception ex)
        {
            activity?.SetTag("status", "error");
            activity?.SetTag("error", ex.Message);
            activity?.SetStatus(Status.Error.WithDescription(ex.Message));

            throw;
        }
    }

    public async Task HandleAsync(LikePostCommand command)
    {
        using var activity = _activitySource.StartActivity($"Handling_{nameof(command)}", ActivityKind.Consumer);

        try
        {
            var aggregate = await _eventSourcingHandler.GetByIdAsync(command.Id);
            aggregate.LikePost();
            await _eventSourcingHandler.SaveAsync(aggregate);

            activity?.SetTag("Status", "Success");
            activity?.SetTag("PostId", command.Id.ToString());
        }
        catch (Exception ex)
        {
            activity?.SetTag("Error", ex.Message);
            activity?.SetStatus(Status.Error.WithDescription(ex.Message));

            throw;
        }
    }
    public async Task HandleAsync(AddCommentCommand command)
    {
        using var activity = _activitySource.StartActivity($"Handling_{nameof(command)}", ActivityKind.Consumer);


        activity?.SetTag("traceId", activity.TraceId);
        activity?.SetTag("parentId", activity.ParentSpanId);

        try
        {
            var aggregate = await _eventSourcingHandler.GetByIdAsync(command.Id);
            aggregate.AddComment(command.Comment, command.UserName);
            await _eventSourcingHandler.SaveAsync(aggregate);
        }
        catch (Exception ex)
        {
            activity?.SetTag("Error", $"Could not find post with Id {command.Id}");
            activity?.SetStatus(Status.Error.WithDescription(ex.Message));

            throw;
        }
    }

    public async Task HandleAsync(EditCommentCommand command)
    {
        var aggregate = await _eventSourcingHandler.GetByIdAsync(command.Id);
        aggregate.EditComment(command.CommentId, command.Comment, command.UserName);
        await _eventSourcingHandler.SaveAsync(aggregate);
    }

    public async Task HandleAsync(RemoveCommentCommand command)
    {
        var aggregate = await _eventSourcingHandler.GetByIdAsync(command.Id);
        aggregate.RemoveComment(command.CommentId, command.UserName);
        await _eventSourcingHandler.SaveAsync(aggregate);
    }

    public async Task HandleAsync(DeletePostCommand command)
    {
        var aggregate = await _eventSourcingHandler.GetByIdAsync(command.Id);
        aggregate.DeletePost(command.UserName);
        await _eventSourcingHandler.SaveAsync(aggregate);
    }

    public async Task HandleAsync(RestoreReadDbCommand command)
    {
        await _eventSourcingHandler.RepublishEventsAsync();
    }
}