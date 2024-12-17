using SharedKernel.Audit;
using SharedKernel.Events.Auth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedKernel.Extensions.Event
{
    public static class EventExtension
    {
        public static string? GetIdCorrelation(object @event)
        {
            return @event is AuditEvent auditEvent && auditEvent.AdditionalData is RegisterErrorEvent registerErrorEvent ? registerErrorEvent.IdCorrelation : null;
        }

        public static string ToEntityName(this AuditEntityType entityType)
        {
            return $"{entityType.ToString().ToLower()}_entity";
        }

        public static string ToOperationType(this AuditOperationType operationType)
        {
            return $"{operationType.ToString().ToLower()}_operation";
        }
    }
}
