namespace SharedKernel.DTO
{
    public class AuditExceptionDTO
    {
        public Exception Exception { get; set; }
        public ApplicationLayer ApplicationLayer { get; set; }
        public ActionType ActionType { get; set; }
        public string AuditTracking { get; set; }
    }
}
