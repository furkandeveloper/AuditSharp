using System.Security.Claims;

namespace AuditSharp.Sample.Middlewares;

public class DefaultUserMiddleware
{
    private readonly RequestDelegate _next;

    public DefaultUserMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        if (context?.User == null || !context.User.Identity?.IsAuthenticated == true)
        {
            // Varsayılan kullanıcıyı oluştur
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, "1"),
                new Claim(ClaimTypes.Name, "DefaultUser"),
                new Claim(ClaimTypes.Role, "DefaultRole")
            };

            var identity = new ClaimsIdentity(claims, "Default");
            context.User = new ClaimsPrincipal(identity);
        }

        await _next(context);
    }
}