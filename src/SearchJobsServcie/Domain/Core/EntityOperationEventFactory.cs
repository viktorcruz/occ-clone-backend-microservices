//using SearchJobsService.Domain.Events;
//using SearchJobsService.Domain.Interface;
//using SharedKernel.Interface;

//namespace SearchJobsService.Domain.Core
//{
//    public class EntityOperationEventFactory : IEntityOperationEventFactory
//    {
//        #region Methods
//        public IAuditEvent CreateEvent(string entityName, string operationType, bool success, string performedBy, string? reason = null, object? additionalData = null)
//        {
//            return new EntityOperationEvent(entityName, operationType, success, performedBy, reason, additionalData);
//        }
//        #endregion
//    }
//}
