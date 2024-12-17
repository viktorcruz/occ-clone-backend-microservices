using SharedKernel.Common.Interfaces.EventBus;
using SharedKernel.Common.Interfaces.Logging;
using SharedKernel.Interfaces.Audit;

namespace SharedKernel.Services
{
    public class EventPublisherService : IEventPublisherService
    {
        private readonly IAuditEventFactory _auditEventFactory;
        private readonly IAsyncEventBus _asyncEventBus;

        public EventPublisherService(IAuditEventFactory auditEventFactory, IAsyncEventBus asyncEventBus)
        {
            _auditEventFactory = auditEventFactory;
            _asyncEventBus = asyncEventBus;
        }

        public async Task PublishEventAsync(string entityName, string operationType, bool success, string performedBy, string reason, object additionalData, string exchangeName = "default_exchange", string routingKey = "default_key")
        {
            var entityEvent = _auditEventFactory.CreateEvent(entityName, operationType, success, performedBy, reason, additionalData);

            await _asyncEventBus.PublishAsyn(exchangeName, routingKey, entityEvent);
        }

        public async Task PublicEventAsync<T>(string exchangeName, string routingKey, T eventMessage)
        {
            await _asyncEventBus.PublishAsyn(exchangeName, routingKey, eventMessage);
        }
    }
}

