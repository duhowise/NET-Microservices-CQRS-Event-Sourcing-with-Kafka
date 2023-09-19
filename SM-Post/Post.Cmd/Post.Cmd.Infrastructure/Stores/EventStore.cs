using System.Data;
using CQRS.Core.Domain;
using CQRS.Core.Events;
using CQRS.Core.Exceptions;
using CQRS.Core.Infrastructure;
using CQRS.Core.Producers;
using Post.Cmd.Domain.Aggregates;

namespace Post.Cmd.Infrastructure.Stores;

public class EventStore:IEventStore
{
    private readonly IEventStoreRepository _eventStoreRepository;
    private readonly IEventProducer _eventProducer;

    public EventStore(IEventStoreRepository eventStoreRepository,IEventProducer eventProducer)
    {
        _eventStoreRepository = eventStoreRepository;
        _eventProducer = eventProducer;
    }

    public async Task SaveEventAsync(Guid aggregateId, IEnumerable<BaseEvent> events, int expectedVersion)
    {
        try
        {
            var eventStream = await _eventStoreRepository.FindByAggregateId(aggregateId);
            if (expectedVersion!=-1 && eventStream[^1].Version!=expectedVersion)
            {
                throw new ConcurrencyException();
            }

            var version = expectedVersion;
            foreach (var @event in events)
            {
                version++;
                @event.Version= version;
                var eventType = @event.GetType().Name;
                var eventModel = new EventModel
                {
                    TimeStamp = DateTime.Now,
                    AggregateIdentifier = aggregateId,
                    AggregateType = nameof(PostAggregate),
                    Version = version,
                    EventType = eventType,
                    EventData = @event
                };

                await _eventStoreRepository.SaveAsync(eventModel);
                await _eventProducer.ProduceAsync(@event);
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    public async Task<List<BaseEvent>> GetEventAsync(Guid aggregateId)
    {
        var eventStream = await _eventStoreRepository.FindByAggregateId(aggregateId);
        if (eventStream==null||!eventStream.Any())
        {
            throw new AggregateNotFoundException("Incorrect Id Provided");
        }

        return eventStream.OrderBy(x => x.Version).Select(e => e.EventData).ToList();
    }
}



