﻿using CQRS.Core.Domain;
using CQRS.Core.Handlers;
using CQRS.Core.Infrastructure;
using CQRS.Core.Producers;
using Post.Cmd.Domain.Aggregates;

namespace Post.Cmd.Infrastructure.Handlers;

public class EventSourcingHandler:IEventSourcingHandler<PostAggregate>
{
    private readonly IEventStore _eventStore;
    private readonly IEventProducer _eventProducer;

    public EventSourcingHandler(IEventStore eventStore,IEventProducer eventProducer)
    {
        _eventStore = eventStore;
        _eventProducer = eventProducer;
    }
    public async Task SaveAsync(AggregateRoot aggregate)
    {
        await _eventStore.SaveEventAsync(aggregate.Id, aggregate.GetUncommitedChanges(), aggregate.Version);
        aggregate.MarkChangesAsCommitted();
    }

    public async Task<PostAggregate> GetByIdAsync(Guid aggregateId)
    {
        var aggregate=new PostAggregate();
        var events = await _eventStore.GetEventAsync(aggregateId);
        if (events==null||!events.Any())
        {
            return aggregate;
        }
        aggregate.ReplayEvents(events);
        aggregate.Version = events.Select(x => x.Version).Max();
        return aggregate;

    }

    public async Task RepublishEventsAsync()
    {
        var aggregateIds = await _eventStore.GetAggregateIdsAsync();
        if (aggregateIds==null||!aggregateIds.Any())
        {
            return;
        }

        foreach (var aggregateId in aggregateIds)
        {
            var aggregate = await GetByIdAsync(aggregateId);
            if (aggregate is not { Active: true })
            {
                continue;
            }
            var events = await _eventStore.GetEventAsync(aggregateId);
            if (events==null||!events.Any())
            {
                continue;
            }

            foreach (var @event in events)
            {
                await _eventProducer.ProduceAsync((dynamic)@event);
            }
        }
    }
}