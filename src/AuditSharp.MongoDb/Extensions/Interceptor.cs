using System.Security.Claims;
using System.Text.Json;
using AuditSharp.Core.Abstracts;
using AuditSharp.Core.Entities;
using AuditSharp.EntityFrameworkCore;
using AuditSharp.EntityFrameworkCore.Context;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.Extensions.DependencyInjection;

namespace AuditSharp.MongoDb.Extensions;

public class Interceptor : SaveChangesInterceptor
{
    private readonly IHttpContextAccessor? _httpContextAccessor;
    private readonly AuditSharpOptions _options;
    private readonly List<TrackedChange> _trackedChanges = new();

    public Interceptor(AuditSharpOptions options)
    {
        _options = options;
        _httpContextAccessor = _options.AuditLogHttpContextAccessor;
    }

    public override ValueTask<int> SavedChangesAsync(SaveChangesCompletedEventData eventData, int result,
        CancellationToken cancellationToken = new())
    {
        TrackChanges(eventData, result);
        return base.SavedChangesAsync(eventData, result, cancellationToken);
    }

    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = new())
    {
        ProcessAsync(eventData);
        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    public override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
    {
        ProcessAsync(eventData);
        return base.SavingChanges(eventData, result);
    }

    private void ProcessAsync(DbContextEventData eventData)
    {
        var context = eventData.Context;

        _trackedChanges.Clear();

        foreach (var entry in context!.ChangeTracker.Entries()
                     .Where(e => e is
                     {
                         Entity: IAuditable, State: EntityState.Added or EntityState.Modified or EntityState.Deleted
                     }))
        {
            var oldValues = new Dictionary<string, object?>();
            var newValues = new Dictionary<string, object?>();

            if (_options.IsIgnoreUnchanged)
            {
                foreach (var property in entry.Properties)
                    if (entry.State is EntityState.Added or EntityState.Deleted || property.IsModified)
                    {
                        oldValues[property.Metadata.Name] =
                            entry.State == EntityState.Added ? null : property.OriginalValue;
                        newValues[property.Metadata.Name] =
                            entry.State == EntityState.Deleted ? null : property.CurrentValue;
                    }
            }
            else
            {
                oldValues = entry.State == EntityState.Added
                    ? null
                    : entry.OriginalValues.Properties.ToDictionary(p => p.Name, p => entry.OriginalValues[p]);

                newValues = entry.State == EntityState.Deleted
                    ? null
                    : entry.CurrentValues.Properties.ToDictionary(p => p.Name, p => entry.CurrentValues[p]);
            }

            _trackedChanges.Add(new TrackedChange(
                entry.Entity.GetType().Name,
                entry.State,
                JsonSerializer.Serialize(oldValues),
                JsonSerializer.Serialize(newValues),
                entry.Metadata.FindPrimaryKey()?.Properties,
                entry
            ));
        }
    }

    public override int SavedChanges(SaveChangesCompletedEventData eventData, int result)
    {
        TrackChanges(eventData, result);
        return base.SavedChanges(eventData, result);
    }

    private int TrackChanges(SaveChangesCompletedEventData eventData, int result)
    {
        if (_trackedChanges.Count == 0) return result;


        var context = eventData.Context;
        if (context == null) return result;

        var auditLogs = new List<AuditLog>();

        foreach (var change in _trackedChanges)
        {
            var primaryKeyValue = change.PrimaryKeyProperties != null
                ? string.Join(",", change.PrimaryKeyProperties.Select(p =>
                    change.Entry.Property(p.Name).CurrentValue?.ToString() ?? "undefined"))
                : "undefined";

            string newValues;
            string oldValues;
            switch (change.State)
            {
                case EntityState.Deleted:
                    oldValues = change.OldValues;
                    newValues = "{}";
                    break;
                case EntityState.Modified:
                    if (_options.IsIgnoreUnchanged)
                    {
                        oldValues = change.OldValues;
                        newValues = change.NewValues;
                    }
                    else
                    {
                        oldValues = change.OldValues;
                        newValues = JsonSerializer.Serialize(
                            change.Entry.CurrentValues.Properties.ToDictionary(p => p.Name,
                                p => change.Entry.OriginalValues[p]));
                    }

                    break;
                case EntityState.Added:
                    oldValues = "{}";
                    newValues = change.NewValues;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            auditLogs.Add(new AuditLog(
                change.EntityName,
                change.State.ToString(),
                oldValues,
                newValues,
                primaryKeyValue,
                _httpContextAccessor?.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "?"
            ));
        }

        var auditSharpContext = IoCManager.Instance.GetRequiredService<IAuditSharpContext>();
        foreach (var log in auditLogs) auditSharpContext.InsertAsync(log);

        _trackedChanges.Clear();
        return result;
    }

    private class TrackedChange(
        string entityName,
        EntityState state,
        string oldValues,
        string newValues,
        IReadOnlyList<IProperty>? primaryKeyProperties,
        EntityEntry entry)
    {
        public string EntityName { get; } = entityName;
        public EntityState State { get; } = state;
        public string OldValues { get; } = oldValues;
        public string NewValues { get; } = newValues;
        public IReadOnlyList<IProperty>? PrimaryKeyProperties { get; } = primaryKeyProperties;

        public EntityEntry Entry { get; } = entry;
    }
}