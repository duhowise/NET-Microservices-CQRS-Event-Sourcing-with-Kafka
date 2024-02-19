using System.Diagnostics;
using CQRS.Core.Events;
using Messaging.Rabbitmq.Interfaces;
using OpenTelemetry.Trace;

namespace Post.Common.Base;

public class QueueConsumer<TEvent> : IQueueConsumer<TEvent> where TEvent : BaseEvent
{
    private readonly IEventHandler _eventHandler;
    private readonly ActivitySource _activitySource = new ActivitySource("QueueConsumer");

    protected QueueConsumer(IEventHandler eventHandler)
    {
        _eventHandler = eventHandler;
    }

    public Task ConsumeAsync(TEvent message)
    {
        using var activity = _activitySource.StartActivity($"Process {nameof(message.GetType)}");

        try
        {
            var handleMethod = _eventHandler.GetType().GetMethod("On", new Type[] { message.GetType() });
            if (handleMethod == null)
            {
                activity?.SetTag("ArgumentNullException","could not find event handler method!");
                throw new ArgumentNullException(nameof(message), "could not find event handler method!");
            }

            handleMethod.Invoke(_eventHandler, new object[] { message });
            activity?.SetStatus(Status.Ok.WithDescription($"Successfully consumed {message.GetType().Name}"));

        }
        catch (Exception ex)
        {
            activity?.SetTag("error", true);
            activity?.SetTag("error.message", ex.Message);
            activity?.SetTag("error.stack", ex.StackTrace);
            activity?.SetStatus(Status.Error.WithDescription(ex.Message));
            throw;
        }

        return Task.CompletedTask;
    }
}