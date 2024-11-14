using FluentValidation.AspNetCore;
using NLog.Web;
using PublicationsService.Aplication.Queries;
using PublicationsService.Modules.Authentication;
using PublicationsService.Modules.Feature;
using PublicationsService.Modules.Injection;
using PublicationsService.Modules.Mapper;
using PublicationsService.Modules.Swagger;
using SharedKernel.Common.Events;
using SharedKernel.Common.Extensions;

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

// Configuración de logging
builder.Logging.ClearProviders();
builder.Logging.ClearProviders();
builder.Host.UseNLog();
builder.Logging.AddConsole();
builder.Logging.SetMinimumLevel(LogLevel.Debug);
builder.Logging.AddFilter("Microsoft.AspNetCore.Authentication", LogLevel.Trace);
builder.Logging.AddFilter("Microsoft.AspNetCore", LogLevel.Debug);

builder.Services.AddEventHandler();

//builder.Services.AddScoped<IEventHandler<UserCreatedEvent>, UserCreatedEventHandler>();
//builder.Services.AddScoped<IEventHandler<UserDeletedEvent>, UserDeletedEventHandler>();
//builder.Services.AddScoped<IEventHandler<PublicationCreatedEvent>, PublicationCreatedEventHandler>();

var app = builder.Build();

app.UseEventRouter()
    .Use(async (context, next) =>
    {
        var eventRouter = context.RequestServices.GetRequiredService<EventRouter>();

        eventRouter.RegisterEventHandler<UserCreatedEvent>(PublicationExchangeNames.User.ToExchangeName(), PublicationRoutingKeys.Created.ToRoutingKey());
        eventRouter.RegisterEventHandler<UserDeletedEvent>(PublicationExchangeNames.User.ToExchangeName(), PublicationRoutingKeys.Deleted.ToRoutingKey());
        eventRouter.RegisterEventHandler<PublicationCreatedEvent>(PublicationExchangeNames.Publication.ToExchangeName(), PublicationRoutingKeys.Created.ToRoutingKey());

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

