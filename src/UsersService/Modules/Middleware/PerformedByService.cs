using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace UsersService.Modules.Middleware
{
    public class PerformedByService
    {
        private readonly RequestDelegate _next;
        private readonly Microsoft.AspNetCore.Http.IHttpContextAccessor _httpContextAccessor;
        private string? _performedBy = string.Empty;

        public PerformedByService(RequestDelegate next, Microsoft.AspNetCore.Http.IHttpContextAccessor httpContextAccessor)
        {
            _next = next;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task Invoke(HttpContext context)
        {
            var authHeader = context.Request.Headers["Authorization"].FirstOrDefault();

            if (!string.IsNullOrEmpty(authHeader) && authHeader.StartsWith("Bearer "))
            {
                var token = authHeader.Substring("Bearer ".Length).Trim();

                try
                {
                    var handler = new JwtSecurityTokenHandler();
                    var jwtToken = handler.ReadToken(token) as JwtSecurityToken;

                    if (jwtToken != null)
                    {
                        var userRole = jwtToken.Claims.FirstOrDefault(c => c.Type == "role" || c.Type == ClaimTypes.Role)?.Value;
                        var userRoleId = jwtToken.Claims.FirstOrDefault(c => c.Type == "sub" || c.Type == ClaimTypes.Name)?.Value;
                        var userEmail = jwtToken.Claims.FirstOrDefault(c => c.Type == "email")?.Value;

                        context.Items["PerformedBy"] = (int.Parse(userRoleId) < 2) ? "Admin" : "Usuario";
                        context.Items["UserEmail"] = !string.IsNullOrEmpty(userEmail) ? userEmail : "NoEmail";
                        context.Items["UserRole"] = !string.IsNullOrEmpty(userRole) ? userRole : "NoRole";
                    }
                }
                catch
                {
                    context.Items["PerformedBy"] = "Anonymous";
                    context.Items["UserEmail"] = "NoEmail";
                    context.Items["UserRole"] = "NoRole";
                }
            }
            else
            {
                context.Items["PerformedBy"] = "Anonymous";
                context.Items["UserEmail"] = "NoEmail";
                context.Items["UserRole"] = "NoRole";
            }


            // registrar el performedBy en context.Items para su uso posterios
            //context.Items["PerformedBy"] = scope;

            // continuar con el siguiente middleware en el pipeline
            await _next(context);
        }
    }
}
