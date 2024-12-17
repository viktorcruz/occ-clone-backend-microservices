using FluentValidation.AspNetCore;
using UsersService.Application.Queries;
using SharedKernel.Modules.Feature;
using UsersService.Modules.Injection;
using UsersService.Modules.Mapper;
using UsersService.Modules.Swagger;
using UsersService.Modules.Authentication;
using NLog.Web;
using SharedKernel.Common.Messaging;
using SharedKernel.Events.JobSearch;
using SharedKernel.Events.User;
using SharedKernel.Extensions.Router;
using SharedKernel.Extensions.Routing;

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
NLog.GlobalDiagnosticsContext.Set("Environment", "Development");
NLog.GlobalDiagnosticsContext.Set("ServerName", Environment.MachineName);
NLog.GlobalDiagnosticsContext.Set("IdCorrelation", Guid.NewGuid().ToString());
NLog.GlobalDiagnosticsContext.Set("MicroserviceName", System.Reflection.Assembly.GetEntryAssembly()?.GetName().Name);

builder.Host.UseNLog();
builder.Logging.AddConsole();
builder.Logging.SetMinimumLevel(Microsoft.Extensions.Logging.LogLevel.Debug);
builder.Logging.AddFilter("Microsoft.AspNetCore.Authentication", Microsoft.Extensions.Logging.LogLevel.Trace);
builder.Logging.AddFilter("Microsoft.AspNetCore", Microsoft.Extensions.Logging.LogLevel.Debug);

builder.Services.AddEventRouter();



var app = builder.Build();

//app.UseEventRouter()
//    .Use(async (context, next) =>
//    {
//        var eventRouter = context.RequestServices.GetRequiredService<EventRouter>();

//        eventRouter.RegisterEventHandler<UserCreatedEvent>(PublicationExchangeNames.Authorize.ToExchangeName(), PublicationRoutingKeys.Register_Success.ToRoutingKey());

//        await next.Invoke();
//    });
app.UseEventRouter()
   .Use(async (context, next) =>
   {
       var eventRouter = context.RequestServices.GetRequiredService<EventRouter>();

       await eventRouter.RegisterEventHandlerAsync<UserCreatedEvent>(
            PublicationExchangeNames.Authorize.ToExchangeName(),
            PublicationRoutingKeys.Register_Success.ToRoutingKey()
        );
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
app.UseMiddleware<SharedKernel.Services.PerformedByService>();
app.UseAuthorization();
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "OCC API V1");
    c.RoutePrefix = string.Empty;
});
app.MapControllers();

app.Run();