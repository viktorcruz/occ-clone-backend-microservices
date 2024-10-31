//using Microsoft.AspNetCore.Authentication.JwtBearer;
//using Microsoft.IdentityModel.Tokens;
//using SharedKernel.Helpers;
//using System.Text;

//namespace AuthService.Modules.Authentication
//{
//    public static class Authentication
//    {
//        public static IServiceCollection AddCustomAuthentication(this IServiceCollection services, IConfiguration configuration)
//        {
//            var appSettingsSection = configuration.GetSection("Jwt");
//            services.Configure<AppSettings>(appSettingsSection);

//            var appSettings = appSettingsSection.Get<AppSettings>();
//            var key = Encoding.ASCII.GetBytes(appSettings.Secret);
//            var issuer = appSettings.Issuer;
//            var audience = appSettings.Audience;

//            services.AddAuthentication(cnf =>
//            {
//                cnf.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
//                cnf.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
//            })
//                .AddJwtBearer(cnf =>
//                {
//                    cnf.Events = new JwtBearerEvents()
//                    {
//                        OnAuthenticationFailed = ctx =>
//                        {
//                            if (ctx.Exception.GetType() == typeof(SecurityTokenExpiredException))
//                            {
//                                ctx.Response.Headers.Add("Token-Expired", "True");
//                            }
//                            return Task.CompletedTask;
//                        }
//                    };
//                    cnf.TokenValidationParameters = new TokenValidationParameters
//                    {
//                        ValidateIssuer = true,
//                        ValidateAudience = true,
//                        ValidateLifetime = true,
//                        ValidateIssuerSigningKey = true,
//                        ValidIssuer = issuer,
//                        ValidAudience = audience,
//                        IssuerSigningKey = new SymmetricSecurityKey(key),
//                        ClockSkew = TimeSpan.Zero
//                    };
//                });
//            return services;

//        }
//    }
//}

