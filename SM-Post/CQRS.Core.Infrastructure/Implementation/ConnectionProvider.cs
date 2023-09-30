using System;
using Messaging.Rabbitmq.Interfaces;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;

namespace Messaging.Rabbitmq.Implementation
{
    public sealed class ConnectionProvider : IDisposable, IConnectionProvider
    {
        private readonly ILogger<ConnectionProvider> _logger;
        private readonly IAsyncConnectionFactory _connectionFactory;
        private IConnection _connection;

        public ConnectionProvider(ILogger<ConnectionProvider> logger, IAsyncConnectionFactory connectionFactory)
        {
            _logger = logger;
            _connectionFactory = connectionFactory;
        }

        public void Dispose()
        {
            try
            {
                if (_connection != null)
                {
                    _logger.LogDebug("Closing the connection");
                    _connection?.Close();
                    _connection?.Dispose();
                }
                _logger.LogDebug("Could not close the connection because it was not created yet");

            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex, "Cannot dispose RabbitMq channel or connection");
            }
        }

        public IConnection GetConnection()
        {
            if (_connection == null || !_connection.IsOpen)
            {
                _logger.LogDebug("Open RabbitMQ connection");
                _connection = _connectionFactory.CreateConnection();
            }

            return _connection;
        }
    }
}