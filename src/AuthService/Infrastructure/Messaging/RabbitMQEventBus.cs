using AuthService.Domain.Ports.Output;
using Dapper;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using SharedKernel.Common.Interfaces;
using SharedKernel.Helpers;
using SharedKernel.Interface;
using System.Text;
using System.Text.Json;

namespace AuthService.Infrastructure.Messaging
{
    public class RabbitMQEventBus : IEventBus
    {
        #region Properties
        private readonly IModel _channel;
        private readonly ILogger<RabbitMQEventBus> _logger;
        private readonly IEventLogPort _eventLogPort;
        #endregion

        #region Constructor
        public RabbitMQEventBus(
            RabbitMQConnection connection,
            ILogger<RabbitMQEventBus> logger,
            IEventLogPort eventLogPort
        )
        {
            _channel = connection.GetChannel();
            _logger = logger;
            _eventLogPort = eventLogPort;
        }
        #endregion

        #region Methods
        public void Publish<T>(string exchange, string routingKey, T @event)
        {
            var message = SerializerEvent.SerializeOrdered((IEntityOperationEvent)@event);

            var body = Encoding.UTF8.GetBytes(message);

            // TODO: declare the exchange and  publish the message
            _channel.ExchangeDeclare(exchange, "direct", durable: true);
            _channel.BasicPublish(exchange, routingKey, null, body);

            // TODO: save the log of the published event in the db
            var parameters = new DynamicParameters();
            parameters.Add("@EventName", "Publish");
            parameters.Add("@EventData", message);
            parameters.Add("@Exchange", exchange);
            parameters.Add("@RoutingKey", routingKey);

            _eventLogPort.SaveEventLog("Usp_EventLog_Add", parameters);

            _logger.LogInformation($"Published event: {message}");
        }


        public void Subscribe<T>(string exchange, string routingKey, Func<T, Task> handler)
        {
            _channel.QueueDeclare(routingKey, durable: false, exclusive: false, autoDelete: false, arguments: null);
            _channel.QueueBind(routingKey, exchange, routingKey);

            var consumer = new EventingBasicConsumer(_channel);
            consumer.Received += async (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                var @event = JsonSerializer.Deserialize<T>(message);

                await handler(@event);
            };

            _channel.BasicConsume(queue: routingKey, autoAck: true, consumer: consumer);
        }
        #endregion
    }
}