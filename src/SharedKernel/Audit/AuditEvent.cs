using SharedKernel.DTO;
using SharedKernel.Interfaces.Audit;

namespace SharedKernel.Audit
{
    public class AuditEvent : IAuditEvent
    {
        public Guid IdEvent => Guid.NewGuid();
        public Guid IdCorrelation => GetIdCorrelation();
        public DateTime OcurredOn => DateTime.UtcNow;

        public string EntityName { get; set; }
        public string OperationType { get; set; }
        public bool Success { get; set; }
        public string Reason { get; set; }
        public string PerfomedBy { get; set; }
        public object AdditionalData { get; set; }

        public AuditEvent(
            string entityName,
            string operationType,
            bool success,
            string performedBy,
            string reason,
            object additionalData
            )
        {
            EntityName = entityName;
            OperationType = operationType;
            Success = success;
            PerfomedBy = performedBy;
            Reason = reason;
            AdditionalData = additionalData;
        }

        public Guid GetIdCorrelation()
        {
            if (AdditionalData is AuditTrackingDTO auditTrackingDTO && Guid.TryParse(auditTrackingDTO.IdCorrelation, out var result))
            {
                return result;
            }
            return Guid.Empty;
        }
    }
}

