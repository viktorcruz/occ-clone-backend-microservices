using SharedKernel.Interfaces.Audit;

namespace SharedKernel.Audit
{
    public class AuditEventFactory : IAuditEventFactory
    {
        public IAuditEvent CreateEvent(string entityName, string operationType, bool success, string performedBy, string reason, object additionalData)
        {
            return new AuditEvent(entityName, operationType,success, performedBy, reason, additionalData);
        }
    }
}