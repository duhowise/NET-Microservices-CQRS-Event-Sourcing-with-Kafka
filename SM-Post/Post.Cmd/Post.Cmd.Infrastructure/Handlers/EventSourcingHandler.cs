using CQRS.Core.Domain;
using CQRS.Core.Handlers;
using CQRS.Core.Infrastructure;
using CQRS.Core.Producers;
using Post.Cmd.Domain.Aggregates;
using System.Diagnostics;
using OpenTelemetry.Trace;

namespace Post.Cmd.Infrastructure.Handlers;

public class EventSourcingHandler : IEventSourcingHandler<PostAggregate>
{
    private readonly IEventStore _eventStore;
    private readonly IEventProducer _eventProducer;
    private static readonly ActivitySource activitySource = new("EventSourcingHandler");

    public EventSourcingHandler(IEventStore eventStore, IEventProducer eventProducer)
    {
        _eventStore = eventStore;
        _eventProducer = eventProducer;
    }

    public async Task SaveAsync(AggregateRoot aggregate)
    {
        using var activity = activitySource.StartActivity("SaveAsync",ActivityKind.Consumer);
        try
        {
            await _eventStore.SaveEventAsync(aggregate.Id, aggregate.GetUncommitedChanges(), aggregate.Version);
            aggregate.MarkChangesAsCommitted();
        }
        catch (Exception ex)
        {
            activity?.SetStatus(Status.Error.WithDescription(ex.Message));
            activity?.AddTag("error.message", ex.Message);
            throw;
        }
    }

    public async Task<PostAggregate> GetByIdAsync(Guid aggregateId)
    {
        using var activity = activitySource.StartActivity("GetByIdAsync");
        try
        {
            var aggregate = new PostAggregate();
            var events = await _eventStore.GetEventAsync(aggregateId);
            if (events == null || !events.Any())
            {
                return aggregate;
            }

            aggregate.ReplayEvents(events);
            aggregate.Version = events.Select(x => x.Version).Max();
            return aggregate;
        }
        catch (Exception ex)
        {
            activity?.SetStatus(Status.Error.WithDescription(ex.Message));
            activity?.AddTag("error.message", ex.Message);

            throw;
        }
    }

    public async Task RepublishEventsAsync()
    {
        using var activity = activitySource.StartActivity("RepublishEventsAsync");
        try
        {
            var aggregateIds = await _eventStore.GetAggregateIdsAsync();
            if (aggregateIds == null || !aggregateIds.Any())
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
                if (events == null || !events.Any())
                {
                    continue;
                }

                foreach (var @event in events)
                {
                    await _eventProducer.ProduceAsync((dynamic)@event);
                }
            }
        }
        catch (Exception ex)
        {
            activity?.SetStatus(Status.Error.WithDescription(ex.Message));
            activity?.AddTag("error.message", ex.Message);
            throw;
        }
    }
}