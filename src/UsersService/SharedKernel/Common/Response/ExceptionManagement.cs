using NLog;
using System.Diagnostics;
using System.Reflection;
using UsersService.SharedKernel.Interface;

namespace UsersService.SharedKernel.Common.Response
{
    public class ExceptionManagement : IExceptionManagement
    {
        public IApiResponse<TResponse> HandleGenericException<TResponse>(Exception ex, string nombreMetodo)
        {
            var logger = LogManager.GetCurrentClassLogger();

            var appBasePath = Directory.GetCurrentDirectory();
            GlobalDiagnosticsContext.Set("appbasepath", appBasePath);

            IApiResponse<TResponse> response = new ApiResponse<TResponse>();
            response.Message = "Error";
            response.IsSuccess = false;

            var logEvent = LogEventInfo.Create(NLog.LogLevel.Error, logger.Name, "Ocurrio un error en " + nombreMetodo);
            logEvent.Properties["LineNumber"] = new StackFrame(0, true).GetFileLineNumber();

            var methodThatThrewException = ex.TargetSite;
            logEvent.Properties["MethodName"] = methodThatThrewException.Name;

            logEvent.Properties["ProyectName"] = Assembly.GetEntryAssembly().GetName().Name;

            logEvent.Exception = ex;
            logger.Log(logEvent);

            return response;
        }
    }
}
