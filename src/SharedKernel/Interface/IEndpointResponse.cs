namespace SharedKernel.Interface
{
    public interface IEndpointResponse<T>
    {
        T Result { get; set; }
        bool IsSuccess { get; set; }
        string Message { get; set; }
    }
}
