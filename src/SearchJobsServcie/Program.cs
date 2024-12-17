using SearchJobsService.Modules.Mapper;
using SearchJobsService.Modules.Feature;
using SearchJobsService.Modules.Injection;
using SearchJobsService.Modules.Authentication;
using SearchJobsService.Modules.Swagger;
using FluentValidation.AspNetCore;
using SearchJobsService.Application.Queries;
using SharedKernel.Common.Interfaces.EventBus;
using SharedKernel.Common.Messaging;
using SearchJobsService.Application.EventListeners.User;
using SearchJobsService.Application.EventListeners.Publication;
using SearchJobsService.Application.EventListeners.Job;
using NLog.Web;
using NLog;
using SharedKernel.Events.JobSearch;
using SharedKernel.Events.Publication;
using SharedKernel.Events.User;
using SharedKernel.Extensions.Router;
using SharedKernel.Extensions.Routing;

var builder = WebApplication.CreateBuilder(args);
var configurationManager = builder.Configuration;
var environment = builder.Environment;

builder.Services.AddMediatR(config =>
{
    config.RegisterServicesFromAssembly(typeof(ApplicationQuery).Assembly);
});

builder.Services.AddControllers()
    .AddFluentValidation(cfg => cfg.RegisterValidatorsFromAssemblyContaining<ApplicationQuery>());


builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Registro de servicios personalizados
builder.Services.AddCustomMapper();
builder.Services.AddCustomFeature(configurationManager);
builder.Services.AddCustomInjection(configurationManager);
builder.Services.AddCustomAuthentication(configurationManager);
builder.Services.AddCustomSwagger();
builder.Services.AddAuthorization();

// Loggin configuration
builder.Logging.ClearProviders();
NLog.GlobalDiagnosticsContext.Set("Environment", "Development");
NLog.GlobalDiagnosticsContext.Set("ServerName", Environment.MachineName);
NLog.GlobalDiagnosticsContext.Set("IdCorrelation", Guid.NewGuid().ToString());
NLog.GlobalDiagnosticsContext.Set("MicroserviceName", System.Reflection.Assembly.GetEntryAssembly()?.GetName().Name);
//NLog.LogManager.Setup().LoadConfigurationFromFile("nlog.config").RegisterNLogWeb();
builder.Host.UseNLog();
builder.Logging.AddConsole();
builder.Logging.SetMinimumLevel(Microsoft.Extensions.Logging.LogLevel.Debug);
builder.Logging.AddFilter("Microsoft.AspNetCore.Authentication", Microsoft.Extensions.Logging.LogLevel.Trace);
builder.Logging.AddFilter("Microsoft.AspNetCore", Microsoft.Extensions.Logging.LogLevel.Debug);

builder.Services.AddEventRouter();

builder.Services.AddScoped<IEventHandler<UserCreatedEvent>, UserCreatedEventHandler>();
builder.Services.AddScoped<IEventHandler<UserUpdatedEvent>, UserUpdatedEventHandler>();
builder.Services.AddScoped<IEventHandler<PublicationCreatedEvent>, PublicationCreatedEventHandler>();
builder.Services.AddScoped<IEventHandler<JobSearchApplyEvent>, SearchJobsApplyEventHandler>();
builder.Services.AddScoped<IEventHandler<PublicationCreationFailedEvent>, PublicationCreationFailedEventHandler>();
var app = builder.Build();

// Registrar eventos en el EventRouter
app.UseEventRouter()
    .Use(async (context, next) =>
    {
        var eventRouter = context.RequestServices.GetRequiredService<EventRouter>();

        // manejadores de eventos especificos de este microservices
        await eventRouter.RegisterEventHandlerAsync<UserCreatedEvent>(PublicationExchangeNames.User.ToExchangeName(), PublicationRoutingKeys.Created.ToRoutingKey());
        await eventRouter.RegisterEventHandlerAsync<UserUpdatedEvent>(PublicationExchangeNames.User.ToExchangeName(), PublicationRoutingKeys.Updated.ToRoutingKey());
        await eventRouter.RegisterEventHandlerAsync<PublicationCreatedEvent>(PublicationExchangeNames.Publication.ToExchangeName(), PublicationRoutingKeys.Created.ToRoutingKey());
        await eventRouter.RegisterEventHandlerAsync<PublicationCreationFailedEvent>(PublicationExchangeNames.Publication.ToExchangeName(), PublicationRoutingKeys.Create_Failed.ToRoutingKey());
        // eventos de saga y otros eventos especificos
        await eventRouter.RegisterEventHandlerAsync<JobApplicationFailedEvent>(
             PublicationExchangeNames.Job.ToExchangeName(),
             PublicationRoutingKeys.Apply_Success.ToRoutingKey()
         );
        await eventRouter.RegisterEventHandlerAsync<JobApplicationFailedEvent>(
             PublicationExchangeNames.Job.ToExchangeName(),
             PublicationRoutingKeys.Apply_Error.ToRoutingKey()
         );
        await eventRouter.RegisterEventHandlerAsync<JobApplicationFailedEvent>(
             PublicationExchangeNames.Job.ToExchangeName(),
             PublicationRoutingKeys.Apply_Failed.ToRoutingKey()
         );
        await next.Invoke();
    });



if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "OCC API V1");
    c.RoutePrefix = string.Empty;
});
app.MapControllers();

app.Run();
