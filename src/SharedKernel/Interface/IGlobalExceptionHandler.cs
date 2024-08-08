namespace SharedKernel.Interface
{
    public interface IGlobalExceptionHandler
    {
        IEndpointResponse<TResponse> HandleGenericException<TResponse>(Exception ex, string methodName);
    }
}
