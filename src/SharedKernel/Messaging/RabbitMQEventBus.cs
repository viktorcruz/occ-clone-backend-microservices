using Dapper;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client.Events;
using RabbitMQ.Client;
using System.Text;
using System.Text.Json;
using SharedKernel.Common.Interfaces.EventBus;
using SharedKernel.Common.Interfaces.Logging;
using SharedKernel.Interfaces.Service;

namespace SharedKernel.Common.Messaging
{
    public class RabbitMQEventBus : IAsyncEventBus
    {
        #region Properties
        private readonly IModel _channel;
        private readonly ILogger<RabbitMQEventBus> _logger;
        private readonly IEventLogStorage _eventLogRepository;
        private readonly ICorrelationService _correlationService;
        #endregion

        #region Constructor
        public RabbitMQEventBus(
                RabbitMQConnection connection,
                ILogger<RabbitMQEventBus> logger,
                IEventLogStorage eventLogRepository,
                ICorrelationService correlationService
            )
        {
            _channel = connection.GetChannel();
            _logger = logger;
            _eventLogRepository = eventLogRepository;
            _correlationService = correlationService;
        }
        #endregion

        #region Methods
        public async Task PublishAsyn<T>(string exchange, string routingKey, T @event)
        {
            _logger.LogInformation($"[RabbitMQ] Publishing event: {@event} to exchange: {exchange} with routing key: {routingKey}");

            try
            {
                var message = JsonSerializer.Serialize(@event);
                var body = Encoding.UTF8.GetBytes(message);

                _channel.ExchangeDeclare(exchange, ExchangeType.Topic, durable: true);

                await Task.Run(() => _channel.BasicPublish(exchange, routingKey, null, body));

                var parameters = new DynamicParameters();
                parameters.Add("@IdCorrelation", _correlationService.GetCorrelationId());
                parameters.Add("@@EventType", "Publish");
                parameters.Add("@EventData", message);
                parameters.Add("@Exchange", exchange);
                parameters.Add("@RoutingKey", routingKey);

                await _eventLogRepository.SaveEventLog("[Usp_IntegrationEvents_Add]", parameters);

                _logger.LogInformation($"[RabbitMQ] Successfully published event: {message}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error publishing event");
                throw;
            }
        }

        public async Task SubscribeAsync<T>(string exchange, string routingKey, Func<T, Task> handler)
        {
            _logger.LogInformation($"[RabbitMQ] Subscribing to exchange: {exchange} with routing key: {routingKey}");

            try
            {
                _channel.ExchangeDeclare(exchange, ExchangeType.Topic, durable: true);
                _channel.QueueDeclare(queue: routingKey, durable: true, exclusive: false, autoDelete: false, arguments: null);
                _channel.QueueBind(queue: routingKey, exchange: exchange, routingKey: routingKey);

                var consumer = new AsyncEventingBasicConsumer(_channel);

                consumer.Received += async (model, ea) =>
                {
                    Console.WriteLine($"[RabbitMQ] Message received on {routingKey}");

                    try
                    {
                        var body = ea.Body.ToArray();
                        var message = Encoding.UTF8.GetString(body);
                        var @event = JsonSerializer.Deserialize<T>(message);

                        Console.WriteLine($"[RabbitMQ] Message Content: {message}");

                        if (@event != null)
                        {
                            _logger.LogInformation($"[RabbitMQ] Event received and deserialized successfully: {message}");
                            Console.WriteLine($"[RabbitMQ] Deserialized event: {@event.GetType().Name}");
                            await handler(@event);
                        }
                        else
                        {
                            _logger.LogWarning($"[RabbitMQ] Event deserialization failed: {message}");
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "[RabbitMQ] Error handling received message");
                        Console.WriteLine("[RabbitMQ] Failed to deserialize message");
                    }
                };

                await Task.Run(() => _channel.BasicConsume(queue: routingKey, autoAck: true, consumer: consumer));

                _logger.LogInformation($"[RabbitMQ] Successfully subscribed to exchange: {exchange} with routing key: {routingKey}");
                Console.WriteLine($"[RabbitMQ] Subscribed to {exchange} with routing key {routingKey}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error subscribing to event");
                throw;
            }
        }
        #endregion
    }
}
