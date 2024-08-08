namespace UsersService.SharedKernel.Interface
{
    public interface IExceptionManagement
    {
        IApiResponse<TResponse> HandleGenericException<TResponse>(Exception ex, string nombreMetodo);
    }
}
