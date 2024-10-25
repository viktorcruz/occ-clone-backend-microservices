using SharedKernel.Interface;
using UsersService.Infrastructure.Interface;

namespace UsersService.Domain.Events
{
    public class EntityOperationEvent : IEvent
    {
        public Guid IdEvent => Guid.NewGuid();
        public DateTime OcurredOn => DateTime.UtcNow;
        public string EntityName { get; set; } // nombre de la entidad afectada: users, publications, ...
        public string OperationType { get; set; } // tipo de operacion: create, update, ...

        public bool Success { get; set; } // indica si al operacion fue exitosa o fallida 
        //public Dictionary<string, object> AddtionalData { get; set; } = new();
        public string Reason { get; set; } // razon del fallo
        //public string UserName { get; set; } //
        public string PerformedBy { get; set; } // nombre del usuario o sistema que ejecuto la operacion
        //public string Email { get; set; }
        public object AdditionalData { get; set; } // cualquier informacion adicional relacionada con la operacion

        public EntityOperationEvent(
            string entityName,
            string operationType,
            bool success,
            string performedBy,
            string reason = null,
            object additionalData = null
        )
        {
            EntityName = entityName;
            OperationType = operationType;
            Success = success;
            PerformedBy = performedBy;
            Reason = reason;
            AdditionalData = additionalData;

        }

        public EntityOperationEvent()
        {

        }
    }
}
