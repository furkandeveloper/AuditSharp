using AuditSharp.EntityFrameworkCore.Context;
using Microsoft.EntityFrameworkCore;

namespace AuditSharp.MongoDb.Context;

public class AuditSharpMongoDbContext : AuditSharpCoreDbContext
{
    protected AuditSharpMongoDbContext()
    {
    }

    public AuditSharpMongoDbContext(DbContextOptions<AuditSharpMongoDbContext> options) : base(options)
    {
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (optionsBuilder.IsConfigured) return;
        Database.AutoTransactionBehavior = AutoTransactionBehavior.Never;
        base.OnConfiguring(optionsBuilder);
    }
}