﻿namespace Messaging.Rabbitmq.Implementation
{
    public class QueueingConfigurationSettings
    {
        public string RabbitMqHostname { get; set; }
        public string RabbitMqUsername { get; set; }
        public string RabbitMqPassword { get; set; }
        public int? RabbitMqPort { get; set; }
        public int? RabbitMqConsumerConcurrency { get; set; }
    }
}