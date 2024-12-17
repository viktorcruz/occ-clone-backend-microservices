namespace SharedKernel.Audit
{
    public class AuditEventEntity
    {
        public string EntityName { get; set; }
        public string OperationType { get; set; }
        public bool Success { get; set; }
        public string PerformedBy { get; set; }
        public string Reason { get; set; }
        public object AdditionalData { get; set; }

        //public AuditEventEntity(string entityName, string operationType, bool success, string performedBy, string reason, string addtionalData)
        //{
        //    EntityName = entityName;
        //    OperationType = operationType;
        //    Success = success;
        //    PerformedBy = performedBy;
        //    Reason = reason;
        //    AdditionalData = addtionalData;
        //}
    }
}
