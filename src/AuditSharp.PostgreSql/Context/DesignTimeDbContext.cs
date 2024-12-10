using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace AuditSharp.PostgreSql.Context;

public class DesignTimeDbContext : IDesignTimeDbContextFactory<AuditSharpPostgreSqlDbContext>
{
    public AuditSharpPostgreSqlDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<AuditSharpPostgreSqlDbContext>();
        optionsBuilder.UseNpgsql("{Your Connection String}");
        return new AuditSharpPostgreSqlDbContext(optionsBuilder.Options);
    }
}