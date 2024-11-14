using AuthService.Modules.Injection;
using NLog;
using NLog.Web;
using SharedKernel.Modules.Authentication;

using SharedKernel.Modules.Feature;
using SharedKernel.Modules.Swagger;

var builder = WebApplication.CreateBuilder(args);
var configurationManager = builder.Configuration;
var environment = builder.Environment;
GlobalDiagnosticsContext.Set("Environment", environment.EnvironmentName);
GlobalDiagnosticsContext.Set("IdCorrelation", Guid.NewGuid().ToString());
//var logger = LogManager.LoadConfiguration("nlog.config").GetCurrentClassLogger();
//var appBasePath = System.IO.Directory.GetCurrentDirectory();
//GlobalDiagnosticsContext.Set("appbasepath", appBasePath);
//var logEvent = LogEventInfo.Create(NLog.LogLevel.Error, logger.Name, "Mensaje de prueba");
//logger.Log(logEvent);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddCustomFeature(configurationManager);
builder.Services.AddCustomInjection();
builder.Services.AddCustomAuthentication(configurationManager);
builder.Services.AddCustomSwagger();

// Configuración de logging
builder.Logging.ClearProviders();
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

app.UseAuthorization();

app.MapControllers();

app.Run();
