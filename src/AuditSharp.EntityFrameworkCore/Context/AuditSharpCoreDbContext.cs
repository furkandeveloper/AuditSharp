using AuditSharp.Core.Entities;
using AuditSharp.EntityFrameworkCore.Generators;
using Microsoft.EntityFrameworkCore;

namespace AuditSharp.EntityFrameworkCore.Context;

public class AuditSharpCoreDbContext : DbContext
{
    protected AuditSharpCoreDbContext()
    {
        
    }

    public AuditSharpCoreDbContext(DbContextOptions options) : base(options)
    {
        
    }
    
    public virtual DbSet<AuditLog> AuditLogs { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<AuditLog>(entity =>
        {
            entity
                .ToTable(nameof(AuditLogs), "audit-sharp");

            entity
                .HasKey(fk => fk.Id);

            entity
                .HasIndex(ix => ix.EntityId);
            entity
                .HasIndex(ix => ix.EntityName);
            entity
                .HasIndex(ix => new { ix.EntityId, ix.EntityName });

            entity
                .Property(p => p.Id)
                .HasValueGenerator<GuidGenerator>()
                .ValueGeneratedOnAdd();

            entity
                .Property(p => p.CreationTime)
                .HasValueGenerator<DateGenerator>()
                .ValueGeneratedOnAdd();

            entity
                .Property(p => p.EntityId)
                .IsRequired();
            entity
                .Property(p => p.EntityName)
                .IsRequired();
            entity
                .Property(p => p.OldValues)
                .IsRequired();
            entity
                .Property(p => p.NewValues)
                .IsRequired();
        });
    }
}