using SharedKernel.Interface;

namespace SearchJobsService.Domain.Events
{
    public class EntityOperationEvent : IEntityOperationEvent
    {
        public Guid IdEvent => Guid.NewGuid();
        public DateTime OcurredOn => DateTime.UtcNow;

        public string EntityName { get; set; }
        public string OperationType { get; set; }
        public bool Success { get; set; }
        public string Reason { get; set; }
        public string PerfomedBy { get; set; }
        public object AdditionalData { get; set; }

        public EntityOperationEvent(
            string entityName = "DefaultEntity",
            string operationType = "DefaultOperation",
            bool success = true,
            string? performedBy = "System",
            string? reason = null,
            object? additionalData = null
            )
        {
            EntityName = entityName;
            OperationType = operationType;
            Success = success;
            PerfomedBy = performedBy;
            Reason = reason;
            AdditionalData = additionalData;
        }
    }
}
