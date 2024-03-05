using CQRS.Core.Events;

namespace Post.Common.Events;

public class CommentUpdatedEvent() : BaseEvent(nameof(CommentUpdatedEvent))
{
    public Guid Id { get; set; }
    public Guid CommentId { get; set; }
    public string Comment { get; set; }
    public string Username { get; set; }
    public DateTime EditDate { get; set; }
}