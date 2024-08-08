namespace UsersService.SharedKernel.Interface
{
    public interface ISpResult
    {
        bool ResultStatus { get; set; }
        string ResultMessage { get; set; }
        string OperationType { get; set; }
        int AffectedRecordId { get; set; }
        DateTime OperationDateTime { get; set; }
        string ExceptionMessage { get; set; }
    }
}
