using Microsoft.Extensions.DependencyInjection;

namespace AuditSharp.PostgreSql.Extensions;

public static class IoCManager
{
    public static IServiceProvider Instance;
    public static void Register(this IServiceCollection services)
    {
        Instance = services.BuildServiceProvider();
    }
}