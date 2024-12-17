using AuthService.Modules.Injection;
using NLog;
using NLog.Web;
using SharedKernel.Modules.Authentication;

using SharedKernel.Modules.Feature;
using SharedKernel.Modules.Swagger;
using SharedKernel.Services;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);
var configurationManager = builder.Configuration;
var environment = builder.Environment;
NLog.GlobalDiagnosticsContext.Set("Environment", environment.EnvironmentName);
NLog.GlobalDiagnosticsContext.Set("ServerName", Environment.MachineName);
NLog.GlobalDiagnosticsContext.Set("IdCorrelation", Guid.NewGuid().ToString());
NLog.GlobalDiagnosticsContext.Set("ProjectName", System.Reflection.Assembly.GetEntryAssembly()?.GetName().Name);

//var logger = LogManager.LoadConfiguration("nlog.config").GetCurrentClassLogger();
//var appBasePath = System.IO.Directory.GetCurrentDirectory();
//GlobalDiagnosticsContext.Set("appbasepath", appBasePath);
//var logEvent = LogEventInfo.Create(NLog.LogLevel.Error, logger.Name, "Mensaje de prueba");
//logger.Log(logEvent);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddCustomFeature(configurationManager);
builder.Services.AddCustomInjection(configurationManager);
builder.Services.AddCustomAuthentication(configurationManager);
builder.Services.AddCustomSwagger();

// Loggin configuration
builder.Logging.ClearProviders();
NLog.GlobalDiagnosticsContext.Set("Environment", "Development");
NLog.GlobalDiagnosticsContext.Set("ServerName", Environment.MachineName);
NLog.GlobalDiagnosticsContext.Set("IdCorrelation", Guid.NewGuid().ToString());
NLog.GlobalDiagnosticsContext.Set("MicroserviceName", System.Reflection.Assembly.GetEntryAssembly()?.GetName().Name);
builder.Host.UseNLog();
builder.Logging.AddConsole();
builder.Logging.SetMinimumLevel(Microsoft.Extensions.Logging.LogLevel.Debug);
builder.Logging.AddFilter("Microsoft.AspNetCore.Authentication", Microsoft.Extensions.Logging.LogLevel.Trace);
builder.Logging.AddFilter("Microsoft.AspNetCore", Microsoft.Extensions.Logging.LogLevel.Debug);

var app = builder.Build();


if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("policy");

app.UseHttpsRedirection();

app.UseMiddleware<PerformedByService>();

app.UseAuthorization();

app.MapControllers();

app.Run();
