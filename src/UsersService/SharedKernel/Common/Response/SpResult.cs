using UsersService.SharedKernel.Interface;

namespace UsersService.SharedKernel.Common.Response
{
    public class SpResult : ISpResult
    {
        public bool ResultStatus { get; set; }
        public string ResultMessage { get; set; } 
        public string OperationType { get; set; }
        public int AffectedRecordId { get; set; }
        public DateTime OperationDateTime { get; set; }
        public string ExceptionMessage { get; set; }
    }

    public class SpRetrieveResult<T> : SpResult
    {
        public T Data { get; set; }
    }
}
