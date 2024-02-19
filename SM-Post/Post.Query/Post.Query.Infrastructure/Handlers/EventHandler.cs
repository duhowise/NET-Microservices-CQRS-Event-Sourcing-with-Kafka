using System.Diagnostics;
using OpenTelemetry.Trace;
using Post.Common.Base;
using Post.Common.Events;
using Post.Query.Domain.Entities;
using Post.Query.Domain.Repositories;

namespace Post.Query.Infrastructure.Handlers;

public class EventHandler:IEventHandler
{
    private readonly IPostRepository _postRepository;
    private readonly ICommentRepository _commentRepository;
    public static readonly ActivitySource ActivitySource = new ActivitySource("EventHandler");
    public EventHandler(IPostRepository postRepository,ICommentRepository commentRepository)
    {
        _postRepository = postRepository;
        _commentRepository = commentRepository;
    }
    public async Task On(PostCreatedEvent @event)
    {
        using var activity = ActivitySource.StartActivity($"Process {nameof(@event)}");
        try
        {
            var post = new PostEntity
            {
                PostId = @event.Id,
                Author = @event.Author,
                DatePosted = @event.DatePosted,
                Message = @event.Message,
            };
            await _postRepository.CreateAsync(post);
            activity?.AddTag("Status", "Post created successfully.");
        }
        catch (Exception ex)
        {
            activity?.SetStatus(Status.Error.WithDescription(ex.Message));
            activity?.AddTag(Status.Error.ToString(), ex.ToString());
            throw;
        }
    }
    public async Task On(MessageUpdatedEvent @event)
    {
        using var activity = ActivitySource.StartActivity($"Process {nameof(@event)}");
        try
        {
            var post = await _postRepository.GetByIdAsync(@event.Id);
            if (post != null)
            {
                post.Message = @event.Message;
            
                activity?.AddTag("message.name", nameof(@event));
            
                await _postRepository.UpdateAsync(post);

                activity?.SetStatus(ActivityStatusCode.Ok, "Message updated successfully");
            }
            else
            {
                activity?.SetStatus(ActivityStatusCode.Error, "Post not found");
            }
        }
        catch (Exception ex)
        {
            activity?.SetStatus(Status.Error.WithDescription(ex.Message));
            throw;
        }
    }

    public async Task On(PostLikedEvent @event)
    {
        using var activity = ActivitySource.StartActivity($"Processing {nameof(@event)}");
        activity?.SetTag("EventId", @event.Id);
        activity?.AddTag("message.name", nameof(@event));

        try
        {
            var post = await _postRepository.GetByIdAsync(@event.Id);
            if (post == null) return;
            post.Likes++;
            await _postRepository.UpdateAsync(post);

            activity?.SetStatus(ActivityStatusCode.Ok, $"Successfully processed {nameof(@event)}");
        }
        catch (Exception ex)
        {
            if (activity is not null)
            {
                activity.SetStatus(ActivityStatusCode.Error, $"{nameof(@event)} processing failed. {ex.Message}");
                activity.RecordException(ex);
            }
        }
    }

    public async Task On(CommentAddedEvent @event)
    {
        // Start a new activity
        using var activity = ActivitySource.StartActivity($"Processing {nameof(@event)}");
        
        activity?.AddTag("UserName", @event.UserName);
        activity?.AddTag("CommentId", @event.CommentId.ToString());

        try
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
            activity?.SetStatus(ActivityStatusCode.Ok, $"Successfully processed {nameof(@event)}");

        }
        catch (Exception ex)
        {
            activity?.SetStatus(Status.Error.WithDescription(ex.Message));
            throw;
        }
    }

    public async Task On(CommentUpdatedEvent @event)
    {
        using var activity = ActivitySource.StartActivity($"Processing {nameof(@event)}");
        activity?.SetTag("EventId", @event.Id);
        activity?.SetTag("Type", "update");
        activity?.SetTag("UserName", @event.UserName);
        activity?.SetTag("CommentId", @event.CommentId.ToString());
        try
        {
            var comment = await _commentRepository.GetByIdAsync(@event.Id);
            if (comment == null)
            {
                activity?.SetStatus(ActivityStatusCode.Error, "No comment found for the provided ID");
                return;
            }
            comment.CommentDate = @event.CommentDate;
            comment.Edited = true;
            await _commentRepository.UpdateAsync(comment);
            activity?.SetStatus(ActivityStatusCode.Ok, $"Successfully processed {nameof(@event)}");

        }
        catch (Exception ex)
        {
            activity?.SetStatus(ActivityStatusCode.Error, $"Exception: {ex.Message}");
            throw;
        }
    }
    public async Task On(CommentRemovedEvent @event)
    {
        // Start a new activity
        using var activity = ActivitySource.StartActivity($"Processing {nameof(@event)}");
    
        // Adding tags
        activity?.AddTag("EventName", nameof(@event));
        activity?.AddTag("CommentId", @event.CommentId.ToString());

        try
        {
            await _commentRepository.DeleteAsync(@event.CommentId);
            activity?.SetStatus(ActivityStatusCode.Ok, $"Successfully processed {nameof(@event)}");

        }
        catch (Exception ex)
        {
            activity?.SetStatus(Status.Error.WithDescription(ex.Message)); // Set the activity status to Error and add the exception in the description.
            throw; // Rethrow the exception.
        }
    }

    public async Task On(PostRemovedEvent @event)
    {
        // Start a new activity for tracking
        using var activity = ActivitySource.StartActivity($"Processing {nameof(@event)}");
    
        // Adding tags
        activity?.AddTag("EventName", nameof(@event));
        activity?.AddTag("EventId", @event.Id.ToString());

        try
        {
            await _postRepository.DeleteAsync(@event.Id);
        }
        catch (Exception ex)
        {
            // Set the status and description in case of exception
            activity?.SetStatus(Status.Error.WithDescription(ex.Message));
            throw;
        }
    }
}