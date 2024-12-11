using System.Text.Json;
using AuditSharp.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace AuditSharp.PostgreSql.Extensions;

public class Interceptor : SaveChangesInterceptor
{
    private readonly List<(EntityEntry Entry, EntityState State)> _trackedChanges = new();

    public override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
    {
        var context = eventData.Context;

        if (context == null) return base.SavingChanges(eventData, result);

        _trackedChanges.Clear();

        foreach (var entry in context.ChangeTracker.Entries().Where(e => 
            e.State == EntityState.Added || e.State == EntityState.Modified || e.State == EntityState.Deleted))
        {
            _trackedChanges.Add((entry, entry.State));
        }

        return base.SavingChanges(eventData, result);
    }

    public override int SavedChanges(SaveChangesCompletedEventData eventData, int result)
    {
        if (_trackedChanges.Count == 0) return 0;

        var context = eventData.Context;
        if (context == null) return 0;

        var auditLogs = new List<AuditLog>();

        foreach (var (entry, state) in _trackedChanges)
        {
            var newValues = string.Empty;
            var oldValues = string.Empty;
            
            if (state == EntityState.Added)
            {
                newValues = JsonSerializer.Serialize(entry.CurrentValues.Properties.ToDictionary(p => p.Name, p => entry.CurrentValues[p]));
            }
            else if (state == EntityState.Deleted)
            {
                oldValues = JsonSerializer.Serialize(entry.OriginalValues.Properties.ToDictionary(p => p.Name, p => entry.OriginalValues[p]));
            }
            else if (state == EntityState.Modified)
            {
                oldValues = JsonSerializer.Serialize(entry.OriginalValues.Properties.ToDictionary(p => p.Name, p => entry.OriginalValues[p]));
                newValues = JsonSerializer.Serialize(entry.CurrentValues.Properties.ToDictionary(p => p.Name, p => entry.CurrentValues[p]));
            }
            auditLogs.Add(new AuditLog(entry.Entity.GetType().Name, entry.State.ToString(), oldValues,newValues, GetPrimaryKeyValue(entry)));
        }

        context.Set<AuditLog>().AddRange(auditLogs);

        return base.SavedChanges(eventData, result);
    }
    // public override int SavedChanges(SaveChangesCompletedEventData eventData, int result)
    // {
    //     var context = eventData.Context;
    //     if(context == null) return base.SavedChanges(eventData, result);
    //     var auditLogs = new List<AuditLog>();
    //     foreach (var entry in context.ChangeTracker.Entries().Where(e => e.State == EntityState.Added || e.State == EntityState.Modified || e.State == EntityState.Deleted))
    //     {
    //         var newValues = string.Empty;
    //         var oldValues = string.Empty;
    //
    //         if (entry.State == EntityState.Added)
    //         {
    //             newValues = JsonSerializer.Serialize(entry.CurrentValues.Properties.ToDictionary(p => p.Name, p => entry.CurrentValues[p]));
    //         }
    //         else if (entry.State == EntityState.Deleted)
    //         {
    //             oldValues = JsonSerializer.Serialize(entry.OriginalValues.Properties.ToDictionary(p => p.Name, p => entry.OriginalValues[p]));
    //         }
    //         else if (entry.State == EntityState.Modified)
    //         {
    //             oldValues = JsonSerializer.Serialize(entry.OriginalValues.Properties.ToDictionary(p => p.Name, p => entry.OriginalValues[p]));
    //             newValues = JsonSerializer.Serialize(entry.CurrentValues.Properties.ToDictionary(p => p.Name, p => entry.CurrentValues[p]));
    //         }
    //         auditLogs.Add(new AuditLog(entry.Entity.GetType().Name, entry.State.ToString(), oldValues,newValues, GetPrimaryKeyValue(entry)));
    //     }
    //     context.Set<AuditLog>().AddRange(auditLogs);
    //     return base.SavedChanges(eventData, result);
    // }

    // public override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
    // {
    //     var context = eventData.Context;
    //     if(context == null) return base.SavingChanges(eventData, result);
    //     var auditLogs = new List<AuditLog>();
    //     foreach (var entry in context.ChangeTracker.Entries().Where(e => e.State == EntityState.Added || e.State == EntityState.Modified || e.State == EntityState.Deleted))
    //     {
    //         var newValues = string.Empty;
    //         var oldValues = string.Empty;
    //
    //         if (entry.State == EntityState.Added)
    //         {
    //             newValues = JsonSerializer.Serialize(entry.CurrentValues.Properties.ToDictionary(p => p.Name, p => entry.CurrentValues[p]));
    //         }
    //         else if (entry.State == EntityState.Deleted)
    //         {
    //             oldValues = JsonSerializer.Serialize(entry.OriginalValues.Properties.ToDictionary(p => p.Name, p => entry.OriginalValues[p]));
    //         }
    //         else if (entry.State == EntityState.Modified)
    //         {
    //             oldValues = JsonSerializer.Serialize(entry.OriginalValues.Properties.ToDictionary(p => p.Name, p => entry.OriginalValues[p]));
    //             newValues = JsonSerializer.Serialize(entry.CurrentValues.Properties.ToDictionary(p => p.Name, p => entry.CurrentValues[p]));
    //         }
    //         auditLogs.Add(new AuditLog(entry.Entity.GetType().Name, entry.State.ToString(), oldValues,newValues, GetPrimaryKeyValue(entry)));
    //     }
    //     context.Set<AuditLog>().AddRange(auditLogs);
    //     return base.SavingChanges(eventData, result);
    // }

    // public override ValueTask<InterceptionResult<int>> SavingChangesAsync(DbContextEventData eventData, InterceptionResult<int> result,
    //     CancellationToken cancellationToken = new CancellationToken())
    // {
    //     SavingChanges(eventData, result);
    //     return base.SavingChangesAsync(eventData, result, cancellationToken);
    // }

    // public override ValueTask<int> SavedChangesAsync(SaveChangesCompletedEventData eventData, int result,
    //     CancellationToken cancellationToken = new CancellationToken())
    // {
    //     SavedChanges(eventData, result);
    //     return base.SavedChangesAsync(eventData, result, cancellationToken);
    // }

    private static string GetPrimaryKeyValue(EntityEntry entry)
    {
        var primaryKey = entry.Metadata.FindPrimaryKey();
        if (primaryKey == null) return "undefined";

        var keyValues = primaryKey.Properties
            .Select(p => entry.Property(p.Name).CurrentValue?.ToString())
            .ToArray();

        return string.Join(",", keyValues);
    }
}