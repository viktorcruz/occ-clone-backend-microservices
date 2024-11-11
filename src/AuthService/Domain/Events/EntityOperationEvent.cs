using SharedKernel.Interface;

namespace AuthService.Domain.Events
{
    public class EntityOperationEvent : IEntityOperationEvent
    {
        public Guid IdEvent { get; }
        public DateTime OcurredOn { get; }

        public string EntityName { get; set; }
        public string OperationType { get; set; }
        public bool Success { get; set; }
        public string Reason { get; set; }
        public string? PerfomedBy { get; set; }
        public object? AdditionalData { get; set; }

        public EntityOperationEvent(
          string entityName = "DefaultEntity",
          string operationType = "DefaultOperation",
          bool success = true,
          string performedBy = "System",
          string? reason = null,
          object? additionalData = null
      )
        {
            IdEvent = Guid.NewGuid();
            OcurredOn = DateTime.Now;
            EntityName = entityName;
            OperationType = operationType;
            Success = success;
            PerfomedBy = performedBy;
            Reason = reason;
            AdditionalData = additionalData;
        }
    }
}
