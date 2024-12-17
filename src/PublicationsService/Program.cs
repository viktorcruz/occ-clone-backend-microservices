using FluentValidation.AspNetCore;
using NLog;
using NLog.Web;
using PublicationsService.Aplication.Queries;
using PublicationsService.Modules.Authentication;
using PublicationsService.Modules.Feature;
using PublicationsService.Modules.Injection;
using PublicationsService.Modules.Mapper;
using PublicationsService.Modules.Swagger;
using SharedKernel.Common.Messaging;
using SharedKernel.Events.JobSearch;
using SharedKernel.Events.Publication;
using SharedKernel.Events.User;
using SharedKernel.Extensions.Router;
using SharedKernel.Extensions.Routing;
using SharedKernel.Services;

var builder = WebApplication.CreateBuilder(args);
var configurationManager = builder.Configuration;
var environment = builder.Environment;

builder.Services.AddMediatR(config =>
{
    config.RegisterServicesFromAssembly(typeof(GetPublicationByIdQuery).Assembly);
});

builder.Services.AddControllers()
    .AddFluentValidation(cfg => cfg.RegisterValidatorsFromAssemblyContaining<GetPublicationByIdQuery>());


builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

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
builder.Host.UseNLog();
builder.Logging.AddConsole();
builder.Logging.SetMinimumLevel(Microsoft.Extensions.Logging.LogLevel.Debug);
builder.Logging.AddFilter("Microsoft.AspNetCore.Authentication", Microsoft.Extensions.Logging.LogLevel.Trace);
builder.Logging.AddFilter("Microsoft.AspNetCore", Microsoft.Extensions.Logging.LogLevel.Debug);

builder.Services.AddEventHandler();

//builder.Services.AddScoped<IEventHandler<UserCreatedEvent>, UserCreatedEventHandler>();
//builder.Services.AddScoped<IEventHandler<UserDeletedEvent>, UserDeletedEventHandler>();
//builder.Services.AddScoped<IEventHandler<PublicationCreatedEvent>, PublicationCreatedEventHandler>();

var app = builder.Build();

app.UseEventRouter()
    .Use(async (context, next) =>
    {
        var eventRouter = context.RequestServices.GetRequiredService<EventRouter>();

        await eventRouter.RegisterEventHandlerAsync<UserCreatedEvent>(PublicationExchangeNames.User.ToExchangeName(), PublicationRoutingKeys.Created.ToRoutingKey());
        await eventRouter.RegisterEventHandlerAsync<UserDeletedEvent>(PublicationExchangeNames.User.ToExchangeName(), PublicationRoutingKeys.Deleted.ToRoutingKey());
        await eventRouter.RegisterEventHandlerAsync<PublicationCreatedEvent>(PublicationExchangeNames.Publication.ToExchangeName(), PublicationRoutingKeys.Created.ToRoutingKey());
        
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
app.UseCors("policy");
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseMiddleware<PerformedByService>();
app.UseAuthorization();
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "OCC API V1");
    c.RoutePrefix = string.Empty;
});
app.MapControllers();

app.Run();

