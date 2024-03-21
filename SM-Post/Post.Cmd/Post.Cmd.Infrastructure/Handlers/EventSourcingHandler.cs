using CQRS.Core.Domain;
using CQRS.Core.Handlers;
using CQRS.Core.Infrastructure;
using CQRS.Core.Producers;
using Post.Cmd.Domain.Aggregates;

namespace Post.Cmd.Infrastructure.Handlers;

public class EventSourcingHandler(IEventStore eventStore, IEventProducer eventProducer) : IEventSourcingHandler<PostAggregate>
{
    public async Task SaveAsync(AggregateRoot aggregate)
    {
        await eventStore.SaveEventsAsync(aggregate.Id, aggregate.GetUncommittedChanges(), aggregate.Version);
        aggregate.MarkChangesAsCommitted();
    }

    public async Task<PostAggregate> GetByIdAsync(Guid aggregateId)
    {
        var aggregate = new PostAggregate();
        var events = await eventStore.GetEventsAsync(aggregateId);

        if (events == null || !events.Any())
            return aggregate;
        
        aggregate.ReplayEvents(events);
        aggregate.Version = events.Select(x => x.Version).Max();

        return aggregate;
    }

    public async Task RepublishEventsAsync()
    {
        var aggregateIds = await eventStore.GetAggregateIdsAsync();

        if (aggregateIds == null || !aggregateIds.Any()) return;

        foreach (var aggregateId in aggregateIds)
        {
            var aggregate = await GetByIdAsync(aggregateId);

            if (aggregate == null || !aggregate.Active) continue;

            var events = await eventStore.GetEventsAsync(aggregateId);
            foreach (var @event in events)
            {
                var topic = Environment.GetEnvironmentVariable("KAFKA_TOPIC");
                await eventProducer.ProduceAsync(topic, @event);
            }
        }
    }
}