using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace AuditSharp.SqlServer.Context;

public class DesignTimeDbContext : IDesignTimeDbContextFactory<AuditSharpSqlServerDbContext>
{
    public AuditSharpSqlServerDbContext CreateDbContext(string[] args)
    {
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json")
            .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")}.json", optional: true)
            .Build();
        var connectionString = configuration.GetConnectionString("DefaultConnection");
        var optionsBuilder = new DbContextOptionsBuilder<AuditSharpSqlServerDbContext>();
        optionsBuilder.UseSqlServer(connectionString);
        return new AuditSharpSqlServerDbContext(optionsBuilder.Options);
    }
}