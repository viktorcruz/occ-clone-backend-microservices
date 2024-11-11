using PublicationsService.Domain.Interface;
using SharedKernel.Common.Interfaces;

namespace PublicationsService.Domain.Services
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

        public async Task PublishEventAsync(string entityName, string operationType, bool success, string performedBy, string? reason = null, object? addtionalData = null, string exchangeName = "default_exchange", string routingKye = "default_key")
        {
            var entityEvent = _eventFactory.CreateEvent(entityName, operationType, success, performedBy, reason, addtionalData);
            _eventBus.Publish(exchangeName, routingKye, entityEvent);
            await Task.CompletedTask;
        }
    }
}
