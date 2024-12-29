using AuditSharp.EntityFrameworkCore.Context;
using Microsoft.EntityFrameworkCore;

namespace AuditSharp.MySql.Context;

public class AuditSharpMySqlDbContext:AuditSharpCoreDbContext
{

    public AuditSharpMySqlDbContext(DbContextOptions<AuditSharpMySqlDbContext> options):base(options)
    {
        
    }
}