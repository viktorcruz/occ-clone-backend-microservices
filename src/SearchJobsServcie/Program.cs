using SearchJobsService.Modules.Mapper;
using SearchJobsService.Modules.Feature;
using SearchJobsService.Modules.Injection;
using SearchJobsService.Modules.Authentication;
using SearchJobsService.Modules.Swagger;
using FluentValidation.AspNetCore;
using SearchJobsService.Application.Queries;
using SharedKernel.Common.Events;
using SharedKernel.Common.Interfaces;
using SharedKernel.Common.Extensions;
using SearchJobsService.Application.EventListeners;
using PublicationsService.Application.EventListeners;

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

// Configuración de logging
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.SetMinimumLevel(LogLevel.Debug);
builder.Logging.AddFilter("Microsoft.AspNetCore.Authentication", LogLevel.Trace);
builder.Logging.AddFilter("Microsoft.AspNetCore", LogLevel.Debug);

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
        eventRouter.RegisterEventHandler<UserCreatedEvent>(PublicationExchangeNames.User.ToExchangeName(), PublicationRoutingKeys.Created.ToRoutingKey());
        eventRouter.RegisterEventHandler<UserUpdatedEvent>(PublicationExchangeNames.User.ToExchangeName(), PublicationRoutingKeys.Updated.ToRoutingKey());
        eventRouter.RegisterEventHandler<PublicationCreatedEvent>(PublicationExchangeNames.Publication.ToExchangeName(), PublicationRoutingKeys.Created.ToRoutingKey());
        eventRouter.RegisterEventHandler<PublicationCreationFailedEvent>(PublicationExchangeNames.Publication.ToExchangeName(), PublicationRoutingKeys.Create_Failed.ToRoutingKey());
        // eventos de saga y otros eventos especificos
        eventRouter.RegisterEventHandler<JobSearchApplyEvent>(PublicationExchangeNames.Job.ToExchangeName(), PublicationRoutingKeys.Apply.ToRoutingKey());

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
