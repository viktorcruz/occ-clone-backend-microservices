//using Dapper;
//using RabbitMQ.Client;
//using RabbitMQ.Client.Events;
//using SharedKernel.Common.Interfaces.EventBus;
//using SharedKernel.Common.Interfaces.Logging;
//using SharedKernel.Extensions.Event;
//using System.Text;
//using System.Text.Json;

//namespace AuthService.Infrastructure.Messaging
//{
//    public class RabbitMQEventBus : IAsyncEventBus
//    {
//        #region Properties
//        private readonly IModel _channel;
//        private readonly ILogger<RabbitMQEventBus> _logger;
//        private readonly IEventLogStorage _eventLogStorage;
//        #endregion

//        #region Constructor
//        public RabbitMQEventBus(
//            RabbitMQConnection connection,
//            ILogger<RabbitMQEventBus> logger,
//            IEventLogStorage eventLogStorage
//        )
//        {
//            _channel = connection.GetChannel();
//            _logger = logger;
//            _eventLogStorage = eventLogStorage;
//        }
//        #endregion

//        #region Methods
//        public async Task PublishAsyn<T>(string exchange, string routingKey, T @event)
//        {
//            _logger.LogInformation($"[AuthService] Publishing event: {@event} to exchange: {exchange} with routing key: {routingKey}");

//            var message = JsonSerializer.Serialize(@event);
//            var body = Encoding.UTF8.GetBytes(message);

//            _channel.ExchangeDeclare(exchange, ExchangeType.Topic, durable: true);

//            // Publicación asíncrona
//            await Task.Run(() => _channel.BasicPublish(exchange, routingKey, null, body));

//            //string? idCorrelation = (@event is IAuditEvent auditEvent && auditEvent.AdditionalData is RegisterErrorEvent registerErrorEvent)
//            //    ? registerErrorEvent.IdCorrelation
//            //    : null;
//            string? idCorrelation = EventExtension.GetIdCorrelation(@event != null);

//            var parameters = new DynamicParameters();
//            parameters.Add("@IdCorrelation", idCorrelation);
//            parameters.Add("@EventName", "Publish");
//            parameters.Add("@EventData", message);
//            parameters.Add("@Exchange", exchange);
//            parameters.Add("@RoutingKey", routingKey);

//            await _eventLogStorage.SaveEventLog("Usp_EventLog_Add", parameters);

//            _logger.LogInformation($"[AuthService] Successfully published event: {message}");
//        }

//        public void Subscribe<T>(string exchange, string routingKey, Func<T, Task> handler)
//        {
//            _channel.QueueDeclare(routingKey, durable: false, exclusive: false, autoDelete: false, arguments: null);
//            _channel.QueueBind(routingKey, exchange, routingKey);

//            var consumer = new EventingBasicConsumer(_channel);
//            consumer.Received += async (model, ea) =>
//            {
//                var body = ea.Body.ToArray();
//                var message = Encoding.UTF8.GetString(body);
//                var @event = JsonSerializer.Deserialize<T>(message);

//                await handler(@event);
//            };

//            _channel.BasicConsume(queue: routingKey, autoAck: true, consumer: consumer);
//        }

//        public Task SubscribeAsync<T>(string exchange, string routingkey, Func<T, Task> handler)
//        {
//            throw new NotImplementedException();
//        }
//        #endregion
//    }
//}