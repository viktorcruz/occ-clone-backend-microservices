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

            _channel.ExchangeDeclare(exchange, "direct", durable: true);
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
            string environmentPrefix = "dev";
            string _routingKey = $"{environmentPrefix}.{routingKey}";
            string exchangeName = $"{environmentPrefix}.{exchange}"; 

#if DEBUG
            _channel.QueueDelete(_routingKey);
#endif
            // Declarar el intercambio con el nombre correcto
            _channel.ExchangeDeclare(
                exchange: exchangeName,  // Usar el nombre correcto del intercambio
                type: ExchangeType.Topic,
                durable: true,
                autoDelete: false,
                arguments: null
            );

            // Declarar la cola usando el nombre completo con prefijo de entorno
            _channel.QueueDeclare(_routingKey, durable: true, exclusive: false, autoDelete: false, arguments: null);

            // Vincular la cola al intercambio correcto
            _channel.QueueBind(
                queue: _routingKey,
                exchange: exchangeName,  // Usar el nombre correcto del intercambio
                routingKey: _routingKey
            );

            var consumer = new EventingBasicConsumer(_channel);
            consumer.Received += async (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                var @event = JsonSerializer.Deserialize<T>(message);
                await handler(@event);
            };

            _channel.BasicConsume(queue: _routingKey, autoAck: true, consumer: consumer);
        }

        public Task PublishAsync<T>(string exchange, string routingKey, T @event)
        {
            return Task.Run(() => Publish(exchange, routingKey, @event));
        }
        #endregion
    }
}
