using Microsoft.Extensions.DependencyInjection;
using SharedKernel.Common.Interfaces;

namespace SharedKernel.Common.Events
{
    public class EventRouter
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly IEventBus _eventBus;

        public EventRouter(IServiceProvider serviceProvider, IEventBus eventBus)
        {
            _serviceProvider = serviceProvider;
            _eventBus = eventBus;
        }

        public void RegisterEventHandler<TEvent>(string exchange, string routingKey) where TEvent : class
        {
            _eventBus.Subscribe<TEvent>(
                exchange,
                routingKey,
                async (eventData) =>
                {
                    // TODO: Resolver el handler adecuado desde el IServiceProvider
                    Console.WriteLine($"Subscription:  {exchange}, {routingKey}");

                    var handler = _serviceProvider.GetRequiredService<IEventHandler<TEvent>>();
                    await handler.Handle(eventData);
                }
            );
        }
    }
}
