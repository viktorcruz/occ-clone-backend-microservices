using FluentValidation.AspNetCore;
using UsersService.Application.Queries;
using SharedKernel.Modules.Feature;
using UsersService.Modules.Injection;
using UsersService.Modules.Mapper;
using UsersService.Modules.Swagger;
using UsersService.Modules.Authentication;
using SharedKernel.Common.Interfaces;
using SharedKernel.Common.Events;

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
builder.Logging.AddConsole();
builder.Logging.SetMinimumLevel(LogLevel.Debug);
builder.Logging.AddFilter("Microsoft.AspNetCore.Authentication", LogLevel.Trace);
builder.Logging.AddFilter("Microsoft.AspNetCore", LogLevel.Debug);

// Construye la aplicacion
var app = builder.Build();

// usa el `ServiceProvider` de `app.Services` para resolver `IEventBus`
var eventBus = app.Services.GetRequiredService<IEventBus>();

// crear y configurar el eventRouter para este microservicio
var eventRouter = new EventRouter(app.Services, eventBus);

// registrar eventos en el eventRouter
eventRouter.RegisterEventHandler<SearchJobsApplyEvent>("search_job_exchange", "search_job.apply");

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