using SharedKernel.Audit;
using SharedKernel.DTO;
using SharedKernel.Extensions.Event;
using SharedKernel.Extensions.Routing;

namespace SharedKernel.Extensions.Audit
{
    public static class AuditDataExtension
    {
        public static object CreatePasswordAuditData(int userId, string email, string performedBy)
        {
            return new
            {
                IdUser = userId,
                Email = email,
                PerformedBy = performedBy,
                Message = "Successfully updated"
            };
        }

        public static AuditEventEntity CreateAuditEvent(object additionalData, string performedBy, bool isTrue, string message) => new AuditEventEntity
        {
            EntityName = AuditEntityType.User.ToEntityName(),
            OperationType = AuditOperationType.ChangePassword.ToOperationType(),
            Success = isTrue ? true : false,
            PerformedBy = performedBy,
            Reason = message,
            AdditionalData = additionalData,
        };

        public static AuditExceptionDTO CreateAuditException(Exception ex, ApplicationLayer layer, ActionType actionType, string auditTracking) => new AuditExceptionDTO
        {
            Exception = ex,
            ApplicationLayer = layer,
            ActionType = actionType,
            AuditTracking = auditTracking,
        };
    }
}