using CQRS.Core.Events;

namespace Post.Common.Events;

public class CommentRemovedEvent() : BaseEvent(nameof(CommentRemovedEvent))
{
    public Guid CommentId { get; set; }
}