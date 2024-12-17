//using SharedKernel.Common.Interfaces.EventBus;
//using SharedKernel.Common.Interfaces.Logging;
//using SharedKernel.Interfaces.Audit;

//namespace SearchJobsService.Domain.Services
//{
//    public class EventPublisherService : IEventPublisherService
//    {
//        private readonly IAuditEventFactory _auditEventFactory;
//        private readonly IAsyncEventBus _eventBus;

//        public EventPublisherService(IAuditEventFactory autitEventFactory, IAsyncEventBus eventBus)
//        {
//            _auditEventFactory = autitEventFactory;
//            _eventBus = eventBus;
//        }


//        public async Task PublishEventAsync(string entityName, string operationType, bool success, string performedBy, string? reason = null, object? additionalData = null, string exchangeName = "default_exchange", string routingKey = "default_key")
//        {
//            var entityEvent = _auditEventFactory.CreateEvent(entityName, operationType, success, performedBy, reason, additionalData);
//            await _eventBus.PublishAsyn(exchangeName, routingKey, entityEvent);
//            await Task.CompletedTask;
//        }

//        public async Task PublicEventAsync<T>(string exchangeName, string routingKey, T eventMessage)
//        {
//            await _eventBus.PublishAsyn(exchangeName, routingKey, eventMessage);
//            await Task.CompletedTask;
//        }
//    }
//}
