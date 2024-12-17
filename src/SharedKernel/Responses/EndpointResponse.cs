using SharedKernel.Interfaces.Response;

namespace SharedKernel.Common.Response
{
    public class EndpointResponse<T> : IEndpointResponse<T>
    {
        #region Properties
        public T Result { get; set; }
        public bool IsSuccess { get; set; }
        public string Message { get; set; }
        #endregion

        #region Constructor
        public EndpointResponse() { }
        public EndpointResponse(T data, bool isSuccess, string message)
        {
            Result = data;
            IsSuccess = isSuccess;
            Message = message;
        }
        #endregion
    }
}
