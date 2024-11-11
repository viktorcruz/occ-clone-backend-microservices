using SearchJobsService.Modules.Mapper;
using SearchJobsService.Modules.Feature;
using SearchJobsService.Modules.Injection;
using SearchJobsService.Modules.Authentication;
using SearchJobsService.Modules.Swagger;
using FluentValidation.AspNetCore;
using SearchJobsService.Application.Queries;
using SharedKernel.Common.Events;
using SharedKernel.Common.Interfaces;

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

// Construye la aplicación
var app = builder.Build();

// Usa el `ServiceProvider` de `app.Services` para resolver `IEventBus`
var eventBus = app.Services.GetRequiredService<IEventBus>();

// Crear y configurar el EventRouter para este microservicio
var eventRouter = new EventRouter(app.Services, eventBus);

// Registrar eventos en el EventRouter
eventRouter.RegisterEventHandler<UserCreatedEvent>("user_exchange", "user.created");
eventRouter.RegisterEventHandler<UserUpdatedEvent>("user_exchange", "user.updated");
eventRouter.RegisterEventHandler<PublicationCreatedEvent>("publication_exchange", "publication.created");


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
