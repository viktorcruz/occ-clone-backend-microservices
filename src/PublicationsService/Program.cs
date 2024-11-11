using FluentValidation.AspNetCore;
using PublicationsService.Aplication.Queries;
using PublicationsService.Modules.Authentication;
using PublicationsService.Modules.Feature;
using PublicationsService.Modules.Injection;
using PublicationsService.Modules.Mapper;
using PublicationsService.Modules.Swagger;
using SharedKernel.Common.Events;
using SharedKernel.Common.Interfaces;

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
builder.Logging.AddConsole();
builder.Logging.SetMinimumLevel(LogLevel.Debug);
builder.Logging.AddFilter("Microsoft.AspNetCore.Authentication", LogLevel.Trace);
builder.Logging.AddFilter("Microsoft.AspNetCore", LogLevel.Debug);

var app = builder.Build();

// Usa el `ServiceProvider` de `app.Services` para resolver `IEventBus`
var eventBus = app.Services.GetRequiredService<IEventBus>();
// Crear y configurar el EventRouter para este microservicio
var eventRouter = new EventRouter(app.Services, eventBus);
// Registrar eventos en el EventRouter
eventRouter.RegisterEventHandler<UserCreatedEvent>("user_exchange", "user.created");
eventRouter.RegisterEventHandler<UserDeletedEvent>("user_exchange", "user.deleted");

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

