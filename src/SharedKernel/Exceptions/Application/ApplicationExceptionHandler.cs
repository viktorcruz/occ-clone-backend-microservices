using NLog;
using SharedKernel.Common.Response;
using SharedKernel.DTO;
using SharedKernel.Interfaces.Exceptions;
using SharedKernel.Interfaces.Response;
using SharedKernel.Interfaces.Service;
using System.Diagnostics;

namespace SharedKernel.Exceptions.Application
{
    public class ApplicationExceptionHandler : IApplicationExceptionHandler
    {
        private readonly ICorrelationService _correlationService;

        public ApplicationExceptionHandler(ICorrelationService correlationService)
        {
            _correlationService = correlationService;
        }
        public IEndpointResponse<TResponse> CaptureException<TResponse>(AuditExceptionDTO capturaDTO)
        {
            var logger = LogManager.GetCurrentClassLogger();
            var idCorrelation = capturaDTO.AuditTracking;

            if (string.IsNullOrWhiteSpace(idCorrelation))
            {
                throw new Exception("idCorrelation esta vacio");
            }
            if (logger == null)
            {
                throw new Exception("NLog no está configurado correctamente. Verifica el archivo nlog.config.");
            }

            // Contexto básico
            var layerName = capturaDTO.ApplicationLayer.ToString();
            var actionName = capturaDTO.ActionType.ToString();
            var appBasePath = Directory.GetCurrentDirectory();
            NLog.GlobalDiagnosticsContext.Set("appbasepath", appBasePath);

            // Crear respuesta de error
            IEndpointResponse<TResponse> response = new EndpointResponse<TResponse>
            {
                IsSuccess = false,
                Message = "Error"
            };

            // Registrar en LogsShared
            var logSharedEvent = CreateLogSharedEvent(capturaDTO.Exception, layerName, actionName, logger, idCorrelation);
            LogShared(logSharedEvent);

            // Registrar en AppLogs
            var appLogEvent = CreateAppLogEvent(capturaDTO.Exception, logger, idCorrelation);
            LogApp(appLogEvent);

            return response;
        }

        public IEndpointResponse<TResponse> CaptureException<TResponse>(Exception ex, ApplicationLayer layer, ActionType action)
        {
            var logger = LogManager.GetCurrentClassLogger();
            if (logger == null)
            {
                throw new Exception("NLog no está configurado correctamente. Verifica el archivo nlog.config.");
            }

            // Contexto básico
            var layerName = layer.ToString();
            var actionName = action.ToString();
            var appBasePath = Directory.GetCurrentDirectory();
            NLog.GlobalDiagnosticsContext.Set("appbasepath", appBasePath);

            // Crear respuesta de error
            IEndpointResponse<TResponse> response = new EndpointResponse<TResponse>
            {
                IsSuccess = false,
                Message = "Error"
            };

            var idCorrelation = _correlationService.GetCorrelationId();

            // Registrar en LogsShared
            var logSharedEvent = CreateLogSharedEvent(ex, layerName, actionName, logger, idCorrelation);
            LogShared(logSharedEvent);

            // Registrar en AppLogs
            var appLogEvent = CreateAppLogEvent(ex, logger, idCorrelation);
            LogApp(appLogEvent);

            return response;
        }

        private void AddCommonLogProperties(Exception ex, Logger logger, string idCorrelation, LogEventInfo logEvent)
        {
            var stackTrace = new StackTrace(ex, true);
            var frame = stackTrace.GetFrame(0);

            logEvent.Properties["IdCorrelation"] = idCorrelation;
            logEvent.Properties["ServerName"] = Environment.MachineName ?? "Unknown";
            logEvent.Properties["FileName"] = frame?.GetFileName() ?? "Unknown";
            logEvent.Properties["LineNumber"] = frame?.GetFileName();
            logEvent.Properties["LogData"] = DateTime.UtcNow;
            logEvent.Properties["ThreadId"] = Thread.CurrentThread.ManagedThreadId;
            logEvent.Properties["Exception"] = ex.ToString();
            logEvent.Properties["Message"] = ex.ToString();
            logEvent.Properties["ExceptionStackTrace"] = ex.StackTrace;

            logEvent.Properties["MicroserviceName"] = NLog.GlobalDiagnosticsContext.Get("MicroserviceName");
            logEvent.Properties["LoggerName"] = logger.Name ?? "DefaultLogger";
            logEvent.Properties["MachineName"] = Environment.MachineName;
            logEvent.Properties["Username"] = Environment.UserName ?? "Unknown";
        }

        private LogEventInfo CreateLogSharedEvent(Exception ex, string layerName, string actionName, Logger logger, string idCorrelation)
        {
            var stackTrace = new StackTrace(ex, true);
            var frame = stackTrace.GetFrame(0);
            var logEvent = LogEventInfo.Create(LogLevel.Error, logger.Name, $"Exception in {layerName}.{actionName}: {ex.Message}");

            AddCommonLogProperties(ex, logger, idCorrelation, logEvent);

            logEvent.Exception = ex;
            logEvent.Properties["Layer"] = layerName;
            logEvent.Properties["Action"] = actionName;
            logEvent.Properties["LogLevel"] = "Error";

            return logEvent;
        }

        private LogEventInfo CreateAppLogEvent(Exception ex, Logger logger, string idCorrelation)
        {
            var stackTrace = new StackTrace(ex, true);
            var frame = stackTrace.GetFrame(0);
            var logEvent = LogEventInfo.Create(LogLevel.Error, logger.Name, "App-specific log");

            AddCommonLogProperties(ex, logger, idCorrelation, logEvent);

            logEvent.Properties["MethodName"] = ex.TargetSite?.Name ?? "Unknown";
            logEvent.Properties["CallSite"] = frame?.GetMethod()?.Name ?? "Unknown";

            return logEvent;
        }

        private void LogShared(LogEventInfo logEvent)
        {
            var logger = LogManager.GetLogger("DatabaseGeneralLog");
            logger.Log(logEvent);
        }

        private void LogApp(LogEventInfo logEvent)
        {
            var logger = LogManager.GetLogger("DatabaseAppLog");
            logger.Log(logEvent);
        }
    }
}
