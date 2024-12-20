using AuditSharp.EntityFrameworkCore;
using AuditSharp.EntityFrameworkCore.Context;
using AuditSharp.PostgreSql.Context;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace AuditSharp.PostgreSql.Extensions;

public static class Program
{
    public static IServiceCollection AddAuditSharp(this IServiceCollection services,
        Action<DbContextOptionsBuilder> optionsBuilder)
    {
        services.AddDbContext<AuditSharpPostgreSqlDbContext>(optionsBuilder, ServiceLifetime.Transient,
            ServiceLifetime.Transient);
        services.AddTransient<IAuditSharpContext>(sp => sp.GetService<AuditSharpPostgreSqlDbContext>()!);
        services.Register();
        return services;
    }

    public static DbContextOptionsBuilder RegisterAuditSharp(this DbContextOptionsBuilder options,
        AuditSharpOptions auditSharpOptions)
    {
        options.AddInterceptors(new Interceptor(auditSharpOptions));
        return options;
    }

    public static IApplicationBuilder UseAuditSharp(this IApplicationBuilder host)
    {
        using var serviceScope = host.ApplicationServices.CreateScope();
        var context = serviceScope.ServiceProvider.GetService<AuditSharpPostgreSqlDbContext>();
        var pendingMigrations = context!.Database.GetPendingMigrations();
        if (pendingMigrations.Any())
            context.Database.Migrate();
        else
            context.Database.EnsureCreated();

        return host;
    }
}