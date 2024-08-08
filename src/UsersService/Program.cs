using FluentValidation.AspNetCore;
using UsersService.Application.Queries;
using UsersService.Modules.Authentication;
using UsersService.Modules.Feature;
using UsersService.Modules.Injection;
using UsersService.Modules.Mapper;
using UsersService.Modules.Swagger;
using UsersService.SharedKernel.Common.Response;
using UsersService.SharedKernel.Interface;

var builder = WebApplication.CreateBuilder(args);
var configurationManager = builder.Configuration;
var environment = builder.Environment;
//var logger = LogManager.LoadConfiguration("nlog.config").GetCurrentClassLogger();
var appBasePath = System.IO.Directory.GetCurrentDirectory();

//var logEvent = LogEventInfo.Create(NLog.LogLevel.Error, logger.Name, "Mensaje de prueba");
//logger.Log(logEvent);
IExceptionManagement exceptionManagement = new ExceptionManagement();
try
{
    throw new Exception("Ocurrió un error personalizado");
}
catch (Exception ex)
{
    var response = exceptionManagement.HandleGenericException<string>(ex, "Program");
}

builder.Services.AddMediatR(config =>
{
    config.RegisterServicesFromAssembly(typeof(GetUserByIdQuery).Assembly);
});

builder.Services.AddControllers()
    .AddFluentValidation(cfg => cfg.RegisterValidatorsFromAssemblyContaining<GetUserByIdQuery>());


builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddCustomMapper();
builder.Services.AddCustomFeature(configurationManager);
builder.Services.AddCustomInjection(configurationManager);
builder.Services.AddCustomAuthentication(configurationManager);
builder.Services.AddCustomSwagger();


var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();