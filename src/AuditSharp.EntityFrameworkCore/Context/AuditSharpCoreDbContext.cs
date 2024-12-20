using System.Linq.Expressions;
using AuditSharp.Core.Entities;
using AuditSharp.EntityFrameworkCore.Generators;
using Microsoft.EntityFrameworkCore;

namespace AuditSharp.EntityFrameworkCore.Context;

public class AuditSharpCoreDbContext : DbContext, IAuditSharpContext
{
    protected AuditSharpCoreDbContext()
    {
    }

    public AuditSharpCoreDbContext(DbContextOptions options) : base(options)
    {
    }

    public virtual DbSet<AuditLog> AuditLogs { get; set; }

    public async Task InsertAsync<T>(T entity, CancellationToken cancellationToken = default) where T : AuditBase
    {
        var entry = Entry(entity);
        entry.State = EntityState.Added;
        await SaveChangesAsync(cancellationToken);
    }

    public IQueryable<T> GetAuditLogsQueryable<T>(Expression<Func<T, bool>>? expression = null) where T : AuditBase
    {
        return Set<T>().AsNoTracking().Where(expression ?? (x => true));
    }

    public IQueryable<T> GetAuditLogsByEntityId<T>(string entityId, string entityName,
        Expression<Func<T, bool>>? expression = null) where T : AuditLog
    {
        return Set<T>().AsNoTracking().Where(w => w.EntityId == entityId && w.EntityName == entityName)
            .Where(expression ?? (x => true));
    }

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