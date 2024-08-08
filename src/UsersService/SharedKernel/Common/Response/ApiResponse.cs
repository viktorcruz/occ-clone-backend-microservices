using UsersService.SharedKernel.Interface;

namespace UsersService.SharedKernel.Common.Response
{
    public class ApiResponse<T> : IApiResponse<T>
    {
        #region Properties
        public T Data { get; set; }
        public bool IsSuccess { get; set; }
        public string Message { get; set; }
        #endregion

        #region Constructor
        public ApiResponse() { }
        public ApiResponse(T data, bool isSuccess, string message)
        {
            Data = data;
            IsSuccess = isSuccess;
            Message = message;
        }
        #endregion
    }
}
