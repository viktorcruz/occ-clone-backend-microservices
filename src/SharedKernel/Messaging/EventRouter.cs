using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SharedKernel.Common.Interfaces.EventBus;

namespace SharedKernel.Common.Messaging
{
    public class EventRouter
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly IAsyncEventBus _eventBus;
        private readonly ILogger<EventRouter> _logger;

        public EventRouter(IServiceProvider serviceProvider, IAsyncEventBus eventBus, ILogger<EventRouter> logger)
        {
            _serviceProvider = serviceProvider;
            _eventBus = eventBus;
            _logger = logger;
        }

        public async Task RegisterEventHandlerAsync<TEvent>(string exchange, string routingKey) where TEvent : class
        {
            _logger.LogInformation($">--; Registering handler for {typeof(TEvent).Name} on exchange: {exchange} with routing key: {routingKey}");

            await _eventBus.SubscribeAsync<TEvent>(
                exchange,
                routingKey,
                async (eventData) =>
                {
                    _logger.LogInformation($">--; Event received for {typeof(TEvent).Name}");

                    try
                    {
                        var handler = _serviceProvider.GetRequiredService<IEventHandler<TEvent>>();
                        await handler.Handle(eventData);
                        _logger.LogInformation($">--; Event handled successfully for {typeof(TEvent).Name}");
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, $">--; Error handling event {typeof(TEvent).Name}");
                    }
                }
            );
            //_logger.LogInformation($">--; LogInformation {typeof(TEvent).Name} from {exchange} with routing key {routingKey}");

            //_eventBus.Subscribe<TEvent>(
            //    exchange,
            //    routingKey,
            //    async (eventData) =>
            //    {
            //        _logger.LogInformation($">--; Received event {typeof(TEvent).Name} from {exchange} with routing key {routingKey}");

            //        try
            //        {
            //            var handler = _serviceProvider.GetRequiredService<IEventHandler<TEvent>>();
            //            await handler.Handle(eventData);
            //            _logger.LogInformation($">--; Handled event {typeof(TEvent).Name} from {exchange} with routing key {routingKey}");
            //        }
            //        catch (Exception ex)
            //        {
            //            _logger.LogError(ex, $">--; Error handling event {typeof(TEvent).Name} from {exchange} with routing key {routingKey}");
            //        }
            //    }
            //);
        }
    }
}
