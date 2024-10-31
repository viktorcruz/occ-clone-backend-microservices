using FluentValidation.AspNetCore;
using UsersService.Application.Queries;
using SharedKernel.Modules.Feature;
using UsersService.Modules.Injection;
using UsersService.Modules.Mapper;
using UsersService.Modules.Swagger;
using UsersService.Modules.Authentication;

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