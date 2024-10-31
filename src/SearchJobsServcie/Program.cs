//var builder = WebApplication.CreateBuilder(args);

//// Add services to the container.

//builder.Services.AddControllers();
//// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
//builder.Services.AddEndpointsApiExplorer();
//builder.Services.AddSwaggerGen();

//var app = builder.Build();

//// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
//{
//    app.UseSwagger();
//    app.UseSwaggerUI();
//}

//app.UseHttpsRedirection();

//app.UseAuthorization();

//app.MapControllers();

//app.Run();

using SearchJobsServcie.Modules.Mapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using SearchJobsServcie.Modules.Feature;
using SearchJobsServcie.Modules.Injection;
using SearchJobsServcie.Modules.Authentication;
using SearchJobsServcie.Modules.Swagger;
using FluentValidation.AspNetCore;
using SearchJobsServcie.Application.Queries;

var builder = WebApplication.CreateBuilder(args);
var configurationManager = builder.Configuration;
var environment = builder.Environment;

builder.Services.AddMediatR(config =>
{
    config.RegisterServicesFromAssembly(typeof(GetJobApplycationsByUserQuery).Assembly);
});

builder.Services.AddControllers()
    .AddFluentValidation(cfg => cfg.RegisterValidatorsFromAssemblyContaining<GetJobApplycationsByUserQuery>());


builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddCustomMapper();
builder.Services.AddCustomFeature(configurationManager);
builder.Services.AddCustomInjection(configurationManager);
builder.Services.AddCustomAuthentication(configurationManager);
builder.Services.AddCustomSwagger();

//var key = configurationManager["JWt:Secret"];
//var issuer = configurationManager["JWT:Issuer"];
//var audience = configurationManager["JWT:Audience"];

//builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
//    .AddJwtBearer("Bearer", options =>
//    {
//        options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
//        {
//            ValidateIssuer = true,
//            ValidateAudience = true,
//            ValidateLifetime = true,
//            ValidateIssuerSigningKey = true,
//            ValidIssuer = issuer,
//            ValidAudience = audience,
//            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key))
//        };
//    });

builder.Services.AddAuthorization();
// logging
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.SetMinimumLevel(LogLevel.Debug);
builder.Logging.AddFilter("Microsoft.AspNetCore.Authentication", LogLevel.Trace);
builder.Logging.AddFilter("Microsoft.AspNetCore", LogLevel.Debug);

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
