using FluentValidation.AspNetCore;
using UsersService.Application.Queries;
using SharedKernel.Modules.Feature;
using UsersService.Modules.Injection;
using UsersService.Modules.Mapper;
using UsersService.Modules.Swagger;
using UsersService.Modules.Authentication;
using SharedKernel.Common.Interfaces;
using SharedKernel.Common.Events;
using SharedKernel.Common.Extensions;
using UsersService.Application.EventListeners;
using NLog.Web;

var builder = WebApplication.CreateBuilder(args);
var configurationManager = builder.Configuration;
var environment = builder.Environment;


builder.Services.AddMediatR(config =>
{
    config.RegisterServicesFromAssembly(typeof(GetUserByIdQuery).Assembly);
});

builder.Services.AddControllers()
    .AddFluentValidation(cfg => cfg.RegisterValidatorsFromAssemblyContaining<GetUserByIdQuery>());


builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Registr de servicios personalizados
builder.Services.AddCustomMapper();
builder.Services.AddCustomFeature(configurationManager);
builder.Services.AddCustomInjection(configurationManager);
builder.Services.AddCustomAuthentication(configurationManager);
builder.Services.AddCustomSwagger();

// Loggin configuration
builder.Logging.ClearProviders();
builder.Host.UseNLog();
builder.Logging.AddConsole();
builder.Logging.SetMinimumLevel(Microsoft.Extensions.Logging.LogLevel.Debug);
builder.Logging.AddFilter("Microsoft.AspNetCore.Authentication", Microsoft.Extensions.Logging.LogLevel.Trace);
builder.Logging.AddFilter("Microsoft.AspNetCore", Microsoft.Extensions.Logging.LogLevel.Debug);

//builder.Services.AddEventBus();
builder.Services.AddEventRouter();

// Registrar manejadores de eventos específicos
builder.Services.AddScoped<IEventHandler<JobSearchApplyEvent>, SearchJobsApplyEventHandler>();
builder.Services.AddScoped<IEventHandler<UserCreatedEvent>, UserCreatedEventHandler>();
builder.Services.AddScoped<IEventHandler<UserUpdatedEvent>, UserUpdatedEventHandler>();
builder.Services.AddScoped<IEventHandler<PublicationCreatedEvent>, PublicationCreatedEventHandler>();

var app = builder.Build();

app.UseEventRouter()
    .Use(async (context, next) =>
    {
        var eventRouter = context.RequestServices.GetRequiredService<EventRouter>();

        eventRouter.RegisterEventHandler<JobSearchApplyEvent>(PublicationExchangeNames.Job.ToExchangeName(), PublicationRoutingKeys.Apply.ToRoutingKey());
        eventRouter.RegisterEventHandler<UserCreatedEvent>(PublicationExchangeNames.User.ToExchangeName(), PublicationRoutingKeys.Created.ToRoutingKey());
        eventRouter.RegisterEventHandler<UserUpdatedEvent>(PublicationExchangeNames.User.ToExchangeName(), PublicationRoutingKeys.Updated.ToRoutingKey());
        eventRouter.RegisterEventHandler<PublicationCreatedEvent>(PublicationExchangeNames.Publication.ToExchangeName(), PublicationRoutingKeys.Created.ToRoutingKey());
        eventRouter.RegisterEventHandler<RegisterSuccessEvent>(PublicationExchangeNames.Authorize.ToExchangeName(), PublicationRoutingKeys.Register_Success.ToRoutingKey());
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
app.UseAuthorization();
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "OCC API V1");
    c.RoutePrefix = string.Empty;
});
app.MapControllers();

app.Run();