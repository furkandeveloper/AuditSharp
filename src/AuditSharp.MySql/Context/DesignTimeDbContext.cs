using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace AuditSharp.MySql.Context;

public class DesignTimeDbContext:IDesignTimeDbContextFactory<AuditSharpMySqlDbContext>
{
    public AuditSharpMySqlDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<AuditSharpMySqlDbContext>();
        optionsBuilder.UseMySql("{Your Connection String}", ServerVersion.AutoDetect("{Your Connection String}"));
        return new AuditSharpMySqlDbContext(optionsBuilder.Options);
    }
}