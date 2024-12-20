using AuditSharp.EntityFrameworkCore;
using AuditSharp.EntityFrameworkCore.Context;
using AuditSharp.MongoDb.Context;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace AuditSharp.MongoDb.Extensions;

public static class Program
{
    public static IServiceCollection AddAuditSharp(this IServiceCollection services,
        Action<DbContextOptionsBuilder> optionsBuilder)
    {
        services.AddDbContext<AuditSharpMongoDbContext>(optionsBuilder, ServiceLifetime.Transient,
            ServiceLifetime.Transient);
        services.AddTransient<IAuditSharpContext>(sp => sp.GetService<AuditSharpMongoDbContext>()!);
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
        return host;
    }
}