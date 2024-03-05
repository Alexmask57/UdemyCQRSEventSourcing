using CQRS.Core.Domain;
using Post.Common.Events;

namespace Post.Cmd.Domain.Aggregates;

public class PostAggregate : AggregateRoot
{
    private string _author;
    private readonly Dictionary<Guid, Tuple<string, string>> _comments = new();

    public bool Active { get; set; }

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
        Id = @event.Id;
        Active = true;
        _author = @event.Author;
    }

    public void EditMessage(string message)
    {
        if (!Active)
        {
            throw new InvalidOperationException("You can't edit the message of an inactive post.");
        }

        if (string.IsNullOrWhiteSpace(message))
        {
            throw new InvalidOperationException(
                $"The value of {nameof(message)} can't be null or empty. Please provide a valid {nameof(message)}.");
        }
        
        RaiseEvent(new MessageUpdatedEvent
        {
            Id = Id,
            Message = message
        });
    }

    public void Apply(MessageUpdatedEvent @event)
    {
        Id = @event.Id;
    }

    public void LikePost()
    {
        if (!Active)
        {
            throw new InvalidOperationException("You can't like an inactive post.");
        }
        
        RaiseEvent(new PostLikedEvent
        {
            Id = Id
        });
    }

    public void Apply(PostLikedEvent @event)
    {
        Id = @event.Id;
    }

    public void AddComment(string comment, string username)
    {
        if (!Active)
        {
            throw new InvalidOperationException("You can't add a comment to an inactive post.");
        }

        if (string.IsNullOrWhiteSpace(comment))
        {
            throw new InvalidOperationException(
                $"The value of {nameof(comment)} can't be null or empty. Please provide a valid {nameof(comment)}");
        }
        
        RaiseEvent(new CommentAddedEvent
        {
            Id = Id,
            CommentId = Guid.NewGuid(),
            Comment = comment,
            Username = username,
            CommentDate = DateTime.Now
        });
    }

    public void Apply(CommentAddedEvent @event)
    {
        Id = @event.Id;
        _comments.Add(@event.CommentId, new Tuple<string, string>(@event.Comment, @event.Username));
    }

    public void EditComment(Guid commentId, string comment, string username)
    {
        if (!Active)
        {
            throw new InvalidOperationException("You can't edit a comment to an inactive post.");
        }

        if (!_comments[commentId].Item2.Equals(username, StringComparison.CurrentCultureIgnoreCase))
        {
            throw new InvalidOperationException(
                "Your are not allowed to edit a comment that was made by another user.");
        }
        
        RaiseEvent(new CommentUpdatedEvent
        {
            Id = Id,
            CommentId = commentId,
            Comment = comment,
            Username = username,
            EditDate = DateTime.Now
        });
    }

    public void Apply(CommentUpdatedEvent @event)
    {
        Id = @event.Id;
        _comments[@event.CommentId] = new Tuple<string, string>(@event.Comment, @event.Username);
    }

    public void RemoveComment(Guid commentId, string username)
    {
        if (!Active)
        {
            throw new InvalidOperationException("You can't delete a comment to an inactive post.");
        }
        
        if (!_comments[commentId].Item2.Equals(username, StringComparison.CurrentCultureIgnoreCase))
        {
            throw new InvalidOperationException(
                "Your are not allowed to delete a comment that was made by another user.");
        }
        
        RaiseEvent(new CommentRemovedEvent
        {
            Id = Id,
            CommentId = commentId
        });
    }

    public void Apply(CommentRemovedEvent @event)
    {
        Id = @event.Id;
        _comments.Remove(@event.CommentId);
    }

    public void DeletePost(string username)
    {
        if (!Active)
        {
            throw new InvalidOperationException("The post has already been removed.");
        }

        if (!_author.Equals(username, StringComparison.CurrentCultureIgnoreCase))
        {
            throw new InvalidOperationException("You are not allowed to delete a post that was made by someone else.");
        }
        
        RaiseEvent(new PostRemovedEvent
        {
            Id = Id
        });
    }

    public void Apply(PostRemovedEvent @event)
    {
        Id = @event.Id;
        Active = false;
    }
}