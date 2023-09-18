using System;
using System.Globalization;
using System.Text;
using Messaging.Rabbitmq.Exceptions;
using Messaging.Rabbitmq.Interfaces;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RabbitMQ.Client;

namespace Messaging.Rabbitmq.Implementation
{
    internal class QueueProducer<TQueueMessage> : IQueueProducer<TQueueMessage> where TQueueMessage : IQueueMessage
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

            if (message.TimeToLive.Ticks <= 0) throw new QueueingException($"{nameof(message.TimeToLive)} cannot be zero or negative");

            // Set message ID
            message.Id = Guid.NewGuid();

            try
            {
                _logger.LogInformation($"Publising message to Queue '{_queueName}' with TTL {message.TimeToLive.TotalMilliseconds}");

                var serializedMessage = SerializeMessage(message);

                var properties = _channel.CreateBasicProperties();
                properties.Persistent = true;
                properties.Type = _queueName;
                properties.Expiration = message.TimeToLive.TotalMilliseconds.ToString(CultureInfo.InvariantCulture);

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
            var stringContent = JsonConvert.SerializeObject(message);
            return Encoding.UTF8.GetBytes(stringContent);
        }
    }
}