namespace SharedKernel.Interface
{
    public interface IApplicationExceptionHandler
    {
        IEndpointResponse<TResponse> CaptureException<TResponse>(Exception ex, ApplicationLayer layer, ActionType action);
    }
}
