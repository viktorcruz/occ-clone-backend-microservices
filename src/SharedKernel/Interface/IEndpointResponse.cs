namespace SharedKernel.Interface
{
    public interface IEndpointResponse<T>
    {
        T Data { get; set; }
        bool IsSuccess { get; set; }
        string Message { get; set; }
    }
}
