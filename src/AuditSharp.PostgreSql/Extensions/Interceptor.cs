using System.Text.Json;
using AuditSharp.Core.Entities;
using AuditSharp.EntityFrameworkCore.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.DependencyInjection;

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
            auditLogs.Add(new AuditLog(entry.Entity.GetType().Name, state.ToString(), oldValues,newValues, GetPrimaryKeyValue(entry)));
        }

        var auditSharpContext = IoCManager.Instance.GetRequiredService<IAuditSharpContext>();
        foreach (var log in auditLogs)
        {
            auditSharpContext.InsertAsync(log);
        }
        return base.SavedChanges(eventData, result);
    }
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