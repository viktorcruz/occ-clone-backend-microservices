namespace SharedKernel.Interface
{
    public interface IEntityOperationEvent : IEvent
    {
        string EntityName { get; }
        string OperationType { get; }
        bool Success { get; }
        string? PerfomedBy { get; }
        string? Reason { get; }
        object? AdditionalData { get; }
    }
}
