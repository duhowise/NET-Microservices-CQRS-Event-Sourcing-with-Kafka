using CQRS.Core.Domain;
using Post.Common.Events;
using System.ComponentModel.Design;

namespace Post.Cmd.Domain.Aggregates;

public class PostAggregate : AggregateRoot
{
    private bool _active;
    private readonly Dictionary<Guid, (string comment, string username)> _comments = new();
    private string _author;

    public bool Active
    {
        get => _active;
        set => _active = value;
    }

    public PostAggregate()
    {
    }

    public PostAggregate(Guid id, string author, string message)
    {
        RaiseEvent(new PostCreatedEvent
        {
            Id = id,
            Author = author,
            Message = message,
            DatePosted = DateTime.Now
        });
    }

    public void Apply(PostCreatedEvent @event)
    {
        _id = @event.Id;
        _active = true;
        _author = @event.Author;
    }

    public void EditMessage(string message)
    {
        if (!_active)
        {
            throw new InvalidOperationException("You cannot edit the message of an inactive post");
        }

        if (string.IsNullOrWhiteSpace(message))
        {
            throw new InvalidOperationException(
                $"the value of {nameof(message)} cannot be null or empty. please provide a valid {nameof(message)}");
        }

        RaiseEvent(new MessageUpdatedEvent
        {
            Id = _id,
            Message = message
        });
    }

    public void Apply(MessageUpdatedEvent @event)
    {
        _id = @event.Id;
    }

    public void Apply(PostLikedEvent @event)
    {
        _id = @event.Id;
    }

    public void LikePost()
    {
        if (!_active)
        {
            throw new InvalidOperationException("You cannot like an inactive post");
        }

        RaiseEvent(new PostLikedEvent
        {
            Id = _id
        });
    }

    public void AddComment(string comment, string username)
    {
        if (!_active)
        {
            throw new InvalidOperationException("You cannot add comment to an inactive post");
        }

        if (string.IsNullOrWhiteSpace(comment))
        {
            throw new InvalidOperationException(
                $"the value of {nameof(comment)} cannot be null or empty. please provide a valid {nameof(comment)}");
        }

        RaiseEvent(new CommentAddedEvent
        {
            Id = _id,
            Comment = comment,
            CommentId = Guid.NewGuid(),
            UserName = username,
            CommentDate = DateTime.Now
        });
    }

    public void Apply(CommentAddedEvent @event)
    {
        _id = @event.Id;
        _comments.Add(@event.CommentId, new(@event.Comment, @event.UserName));
    }

    public void EditComment(Guid commentId, string comment, string username)
    {
        if (!_active)
        {
            throw new InvalidOperationException("You cannot edit comment to an inactive post");
        }

        if (!_comments[commentId].username.Equals(username, StringComparison.CurrentCultureIgnoreCase))
        {
            throw new InvalidOperationException("You are not allowed to edit a comment that was made by another user.");
        }

        RaiseEvent(new CommentUpdatedEvent()
        {
            Id = _id,
            Comment = comment,
            CommentId = commentId,
            UserName = username,
            CommentDate = DateTime.Now
        });
    }

    public void Apply(CommentUpdatedEvent @event)
    {
        _id = @event.Id;
        _comments[@event.CommentId] = new(@event.Comment, @event.UserName);
    }

    public void RemoveComment(Guid commentId, string username)
    {
        if (!_active)
        {
            throw new InvalidOperationException("You cannot remove comment to an inactive post");
        }

        if (!_comments[commentId].username.Equals(username, StringComparison.CurrentCultureIgnoreCase))
        {
            throw new InvalidOperationException(
                "You are not allowed to remove a comment that was made by another user.");
        }

        RaiseEvent(new CommentRemovedEvent()
        {
            Id = _id,
            CommentId = commentId,
        });
    }

    public void Apply(CommentRemovedEvent @event)
    {
        _id = @event.Id;
        _comments.Remove(@event.CommentId);
    }


    public void DeletePost(string userName)
    {
        if (!_active)
        {
            throw new InvalidOperationException("The post has already been removed");
        }

        if (!_author.Equals(userName, StringComparison.CurrentCultureIgnoreCase))
        {
            throw new InvalidOperationException("You are not allowed to remove a post that was made by another user.");
        }

        RaiseEvent(new PostRemovedEvent
        {
            Id = _id,
        });
    }

    public void Apply(PostRemovedEvent @event)
    {
        _id = @event.Id;
        _active = false;
    }
}