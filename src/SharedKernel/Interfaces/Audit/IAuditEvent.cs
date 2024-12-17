using SharedKernel.Interfaces.Event;

namespace SharedKernel.Interfaces.Audit
{
    public interface IAuditEvent : IEvent
    {
        string EntityName { get; }
        string OperationType { get; }
        bool Success { get; }
        string PerfomedBy { get; }
        string Reason { get; }
        object AdditionalData { get; }
    }
}

