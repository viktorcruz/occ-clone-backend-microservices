using SharedKernel.Interfaces.Response;

namespace SharedKernel.Common.Responses
{
    public class DatabaseResult : IDatabaseResult
    {
        public bool ResultStatus { get; set; }
        public string ResultMessage { get; set; }
        public string OperationType { get; set; }
        public int AffectedRecordId { get; set; }
        public DateTime OperationDateTime { get; set; }
        public string ExceptionMessage { get; set; }
    }

    public class RetrieveDatabaseResult<T> : DatabaseResult
    {
        public T Details { get; set; }
    }
}
