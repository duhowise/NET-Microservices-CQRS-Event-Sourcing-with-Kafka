using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CQRS.Core.Events;

namespace CQRS.Core.Infrastructure;

public interface IEventStore
{
    Task SaveEventAsync(Guid aggregateId, IEnumerable<BaseEvent> events, int expectedVersion);
    Task<List<BaseEvent>> GetEventAsync(Guid aggregateId);
    Task<List<Guid>> GetAggregateIdsAsync();
}