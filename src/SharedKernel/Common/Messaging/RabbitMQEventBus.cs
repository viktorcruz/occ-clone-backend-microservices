using Dapper;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client.Events;
using RabbitMQ.Client;
using SharedKernel.Interface;
using System.Text;
using System.Text.Json;
using SharedKernel.Common.Interfaces;
using SharedKernel.Helpers;

namespace SharedKernel.Common.Messaging
{
    public class RabbitMQEventBus : IEventBus
    {
        #region Properties
        private readonly IModel _channel;
        private readonly ILogger<RabbitMQEventBus> _logger;
        private readonly IEventLogRepository _eventLogRepository;
        #endregion

        #region Constructor
        public RabbitMQEventBus(
                RabbitMQConnection connection,
                ILogger<RabbitMQEventBus> logger,
                IEventLogRepository eventLogRepository
            )
        {
            _channel = connection.GetChannel();
            _logger = logger;
            _eventLogRepository = eventLogRepository;
        }
        #endregion

        #region Methods
        public void Publish<T>(string exchange, string routingKey, T @event)
        {
            var message = SerializerEvent.SerializeOrdered((IEntityOperationEvent)@event);
            var body = Encoding.UTF8.GetBytes(message);

            _channel.ExchangeDeclare(exchange, ExchangeType.Topic, durable: true);
            _channel.BasicPublish(exchange, routingKey, null, body);

            var parameters = new DynamicParameters();
            parameters.Add("@EventName", "Publish");
            parameters.Add("@EventData", message);
            parameters.Add("@Exchange", exchange);
            parameters.Add("@RoutingKey", routingKey);

            _eventLogRepository.SaveEventLog("Usp_EventLog_Add", parameters);

            _logger.LogInformation($"Published event: {message}");
        }

        public void Subscribe<T>(string exchange, string routingKey, Func<T, Task> handler)
        {
            var consumer = new AsyncEventingBasicConsumer(_channel);

            consumer.Received += async (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                var @event = JsonSerializer.Deserialize<T>(message);

                if (@event != null)
                {
                    await handler(@event);
                }
            };

            _channel.ExchangeDeclare(exchange, ExchangeType.Topic, durable: true);
            _channel.QueueDeclare(queue: routingKey, durable: true, exclusive: false, autoDelete: false, arguments: null);
            _channel.QueueBind(queue:routingKey, exchange: exchange, routingKey: routingKey);

            _channel.BasicConsume(queue: routingKey, autoAck:true, consumer: consumer);

            _logger.LogInformation($"Subscribed to exchange: {exchange} with routing key {routingKey}");
        }

        public Task PublishAsync<T>(string exchange, string routingKey, T @event)
        {
            return Task.Run(() => Publish(exchange, routingKey, @event));
        }
        #endregion
    }
}
