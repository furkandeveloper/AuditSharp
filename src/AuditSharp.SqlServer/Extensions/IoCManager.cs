using Microsoft.Extensions.DependencyInjection;

namespace AuditSharp.SqlServer.Extensions;

public static class IoCManager
{
    public static IServiceProvider Instance;
    public static void Register(this IServiceCollection services)
    {
        Instance = services.BuildServiceProvider();
    }
}
