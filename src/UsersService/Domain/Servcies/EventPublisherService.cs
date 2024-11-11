using SharedKernel.Common.Interfaces;
using UsersService.Domain.Interface;

namespace UsersService.Domain.Servcies
{
    public class EventPublisherService : IEventPublisherService
    {
        private readonly IEntityOperationEventFactory _eventFactory;
        private readonly IEventBus _eventBus;

        public EventPublisherService(IEntityOperationEventFactory eventFactory, IEventBus eventBus)
        {
            _eventFactory = eventFactory;
            _eventBus = eventBus;
        }

        public async Task PublishEventAsync(string entityName, string operationType, bool success, string performedBy, string? reason = null, object? additionalData = null, string exchangeName = "default_exchange", string routingKey = "default_key")
        {
            var entityEvent = _eventFactory.CreateEvent(entityName, operationType, success, performedBy, reason, additionalData);
            _eventBus.Publish(exchangeName, routingKey, entityEvent);
            await Task.CompletedTask;
        }
    }
}
