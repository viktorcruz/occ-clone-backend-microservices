using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;
using UsersService.Infrastructure.Interface;


namespace UsersService.Infrastructure.Messaging
{
    public class EventBusRabbitMQ
    {
        private readonly IModel _channel;
        private readonly ILogger<EventBusRabbitMQ> _logger;
        private readonly IEventLogRepository _eventLogRepository;

        public EventBusRabbitMQ(
            RabbitMQConnection connection,
            ILogger<EventBusRabbitMQ> logger,
            IEventLogRepository eventLogRepository)
        {
            _channel = connection.GetChannel();
            _logger = logger;
            _eventLogRepository = eventLogRepository;
        }

        public void Publish(string exchange, string routingKey, object eventMessage)
        {
            var body = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(eventMessage));
            _channel.ExchangeDeclare(exchange, "direct");
            _channel.BasicPublish(exchange, routingKey, null, body);

            _logger.LogInformation($"Event published to exchange '{exchange}' with routing key '{routingKey}': {JsonSerializer.Serialize(eventMessage)}");
            _eventLogRepository.SaveEventLog("Publish", JsonSerializer.Serialize(eventMessage), exchange, routingKey);

        }

        public void Subscribe<T>(string queue, string exchange, string routingKey, Action<T> handleMessage)
        {
            _channel.QueueDeclare(queue, true, false, false, null);
            _channel.QueueBind(queue, exchange, routingKey);

            var consumer = new EventingBasicConsumer(_channel);
            consumer.Received += (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                var eventMessage = JsonSerializer.Deserialize<T>(message);

                _logger.LogInformation($"Event reaceived from exchange '{exchange}' with routing key '{routingKey}': {message}");
                _eventLogRepository.SaveEventLog("Subscribe", message, exchange, routingKey);

                handleMessage(eventMessage);
            };

            _channel.BasicConsume(queue, true, consumer);
        }


    }
}