using AuthService.Domain.Events;
using AuthService.Domain.Interfaces;
using SharedKernel.Interface;

namespace AuthService.Domain.Core
{
    public class EntityOperationEventFactory : IEntityOperationEventFactory
    {
        public IEntityOperationEvent CreateEvent(string entityName, string operationType, bool success, string performedBy, string? reason = null, object? additionalData = null)
        {
            return new EntityOperationEvent(entityName, operationType, success, performedBy, reason, additionalData);
        }
    }
}
