namespace SharedKernel.Interfaces.Audit
{
    public interface IAuditEventFactory
    {
        IAuditEvent CreateEvent(
            string entityName,
            string operationType,
            bool success,
            string performedBy,
            string reason,
            object additionalData 
        );
    }
}