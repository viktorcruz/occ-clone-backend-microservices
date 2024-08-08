namespace UsersService.SharedKernel.Interface
{
    public interface IApiResponse<T>
    {
        T Data { get; set; }
        bool IsSuccess { get; set; }
        string Message { get; set; }
    }
}
