using AuditSharp.EntityFrameworkCore.Context;
using Microsoft.EntityFrameworkCore;

namespace AuditSharp.PostgreSql.Context;

public class AuditSharpPostgreSqlDbContext : AuditSharpCoreDbContext
{
    protected AuditSharpPostgreSqlDbContext()
    {
        
    }
    
    public AuditSharpPostgreSqlDbContext(DbContextOptions<AuditSharpPostgreSqlDbContext> options) : base(options)
    {
        
    }
}