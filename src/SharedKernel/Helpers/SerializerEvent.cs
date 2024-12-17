using SharedKernel.Interfaces.Audit;
using System.Text.Json;

namespace SharedKernel.Helpers
{
    public static class SerializerEvent
    {
        public static string SerializeOrdered(IAuditEvent @event)
        {
            var orderedEvent = new
            {
                @event.IdEvent,
                @event.OcurredOn,
                @event.EntityName,
                @event.OperationType,
                @event.Success,
                @event.PerfomedBy,
                @event.Reason,
                @event.AdditionalData,
            };

            return JsonSerializer.Serialize(orderedEvent, new JsonSerializerOptions { WriteIndented = false });
        }
    }
}
