using SharedKernel.Interface;

namespace SearchJobsService.Domain.Interface
{
    public interface IEntityOperationEventFactory
    {
        IEntityOperationEvent CreateEvent(
            string entityName,
            string operationType,
            bool success,
            string performedBy,
            string? reason = null,
            object? additionalData = null
            );
    }
}
