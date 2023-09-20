using System.Globalization;
using System.Text;
using System.Text.Json;
using Messaging.Rabbitmq.Exceptions;
using Messaging.Rabbitmq.Interfaces;
using Microsoft.Extensions.Logging;
using MongoDB.Bson.IO;
using RabbitMQ.Client;

namespace Post.Common.Queue
{
    public class QueueProducer<TQueueMessage> : IQueueProducer<TQueueMessage> where TQueueMessage : IQueueMessage
    {
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

            try
            {
                _logger.LogInformation("Publising message to Queue {QueueName}",_queueName);

                var serializedMessage = SerializeMessage(message);

                var properties = _channel.CreateBasicProperties();
                properties.Persistent = true;
                properties.Type = _queueName;
                _channel.BasicPublish(_queueName, _queueName, properties, serializedMessage);

                _logger.LogDebug(
                    $"Succesfully published message");
            }
            catch (Exception ex)
            {
                var msg = $"Cannot publish message to Queue '{_queueName}'";
                _logger.LogError(ex, msg);
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