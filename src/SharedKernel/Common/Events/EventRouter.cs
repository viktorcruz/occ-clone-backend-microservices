using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SharedKernel.Common.Interfaces;

namespace SharedKernel.Common.Events
{
    public class EventRouter
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly IEventBus _eventBus;
        private readonly ILogger<EventRouter> _logger;

        public EventRouter(IServiceProvider serviceProvider, IEventBus eventBus, ILogger<EventRouter> logger)
        {
            _serviceProvider = serviceProvider;
            _eventBus = eventBus;
            _logger = logger;
        }

        public void RegisterEventHandler<TEvent>(string exchange, string routingKey) where TEvent : class
        {
            _eventBus.Subscribe<TEvent>(
                exchange,
                routingKey,
                async (eventData) =>
                {
                    try
                    {
                        var handler = _serviceProvider.GetRequiredService<IEventHandler<TEvent>>();
                        await handler.Handle(eventData);
                        _logger.LogInformation($"Handled event {typeof(TEvent).Name} from {exchange} with routing key {routingKey}");
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, $"Error handling event {typeof(TEvent).Name} from {exchange} with routing key {routingKey}");
                    }
                }
            );
        }
    }
}
