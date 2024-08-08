using Microsoft.Extensions.Logging;
using NLog;
using SharedKernel.Common.Response;
using SharedKernel.Interface;
using System.Diagnostics;
using System.Reflection;

namespace SharedKernel.Common.Exceptions
{
    public class GlobalExceptionHandler : IGlobalExceptionHandler
    {
        //public IEndpointResponse<TResponse> HandleGenericException<TResponse>(Exception ex, string nombreMetodo)
        //{
        //    var logger = LogManager.GetCurrentClassLogger();

        //    var appBasePath = Directory.GetCurrentDirectory();
        //    GlobalDiagnosticsContext.Set("appbasepath", appBasePath);

        //    IEndpointResponse<TResponse> response = new EndpointResponse<TResponse>();
        //    response.Message = "Error";
        //    response.IsSuccess = false;

        //    var logEvent = LogEventInfo.Create(NLog.LogLevel.Error, logger.Name, "Ocurrio un error en " + nombreMetodo);
        //    logEvent.Properties["LineNumber"] = new StackFrame(0, true).GetFileLineNumber();

        //    var methodThatThrewException = ex.TargetSite;
        //    logEvent.Properties["MethodName"] = methodThatThrewException.Name;

        //    logEvent.Properties["ProyectName"] = Assembly.GetEntryAssembly().GetName().Name;

        //    logEvent.Exception = ex;
        //    logger.Log(logEvent);

        //    return response;
        //}

        private readonly ILogger<GlobalExceptionHandler> _logger;

        public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger)
        {
            _logger = logger;
        }

        public IEndpointResponse<TResponse> HandleGenericException<TResponse>(Exception ex, string methodName)
        {
            var response = new EndpointResponse<TResponse>
            {
                Message = "Error",
                IsSuccess = false
            };

            LogException(ex, methodName);

            return response;
        }

        private void LogException(Exception ex, string methodName)
        {
            var logEvent = new LogEventInfo(NLog.LogLevel.Error, _logger.GetType().FullName, "An error ocurred in " + methodName);
            logEvent.Properties["LineNumber"] = new StackFrame(0, true).GetFileLineNumber();
            logEvent.Properties["MethodName"] = ex.TargetSite.Name;
            logEvent.Properties["ProjectName"] = Assembly.GetEntryAssembly().GetName().Name;
            logEvent.Exception = ex;

            LogManager.GetCurrentClassLogger().Log(logEvent);
        }
    }
}
