using AuditSharp.EntityFrameworkCore.Context;
using Microsoft.EntityFrameworkCore;

namespace AuditSharp.SqlServer.Context;

public class AuditSharpSqlServerDbContext : AuditSharpCoreDbContext
{
    protected AuditSharpSqlServerDbContext()
    {
        
    }
    
    public AuditSharpSqlServerDbContext(DbContextOptions<AuditSharpSqlServerDbContext> options) : base(options)
    {
        
    }
}