using System.Text;
using System.Text.Json;
using System.Diagnostics;
using Messaging.Rabbitmq.Exceptions;
using Messaging.Rabbitmq.Interfaces;
using Microsoft.Extensions.Logging;
using OpenTelemetry.Trace;
using RabbitMQ.Client;

namespace Post.Common.Queue
{
    public class QueueProducer<TQueueMessage> : IQueueProducer<TQueueMessage> where TQueueMessage : IQueueMessage
    {
        private readonly ActivitySource _activitySource = new("QueueProducer");
        private readonly ILogger<QueueProducer<TQueueMessage>> _logger;
        private readonly string _queueName;
        private readonly IModel _channel;

        public QueueProducer(IQueueChannelProvider<TQueueMessage> channelProvider, ILogger<QueueProducer<TQueueMessage>> logger)
        {
            _logger = logger;
            _channel = channelProvider.GetChannel();
            _queueName = typeof(TQueueMessage).Name;
        }

        public void PublishMessage(TQueueMessage message)
        {
            if (Equals(message, default(TQueueMessage))) throw new ArgumentNullException(nameof(message));

            using var activity = _activitySource.StartActivity("PublishMessage");
            
            try
            {
                _logger.LogInformation("Publishing message to Queue {QueueName}", _queueName);

                var serializedMessage = SerializeMessage(message);

                var properties = _channel.CreateBasicProperties();
                properties.Persistent = true;
                properties.Type = _queueName;
                _channel.BasicPublish(_queueName, _queueName, properties, serializedMessage);

                _logger.LogDebug("Successfully published message");
                activity?.AddTag("message.status", "Published successfully");
            }
            catch (Exception ex)
            {
                var msg = $"Cannot publish message to Queue '{_queueName}'";
                _logger.LogError(ex, msg);
                activity?.SetStatus(Status.Error.WithDescription(ex.Message));
                activity?.AddTag("error.message", ex.Message);
                throw new QueueingException(msg);
            }
        }

        private static byte[] SerializeMessage(TQueueMessage message)
        {
            var stringContent = JsonSerializer.Serialize(message,new JsonSerializerOptions()
            {
                Converters = { new Post.Common.Converter.EventJsonConverter() }
            });
            return Encoding.UTF8.GetBytes(stringContent);
        }
    }
}