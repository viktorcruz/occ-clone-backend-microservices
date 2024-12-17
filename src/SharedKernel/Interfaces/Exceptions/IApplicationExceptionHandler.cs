using SharedKernel.DTO;
using SharedKernel.Interfaces.Response;

namespace SharedKernel.Interfaces.Exceptions
{
    public interface IApplicationExceptionHandler
    {
        IEndpointResponse<TResponse> CaptureException<TResponse>(Exception ex, ApplicationLayer layer, ActionType action);
        IEndpointResponse<TResponse> CaptureException<TResponse>(AuditExceptionDTO capturaDTO);
    }
}
