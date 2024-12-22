using System.Linq.Expressions;
using AuditSharp.Core.Entities;

namespace AuditSharp.EntityFrameworkCore.Context;

public interface IAuditSharpContext
{
    Task InsertAsync<T>(T entity, CancellationToken cancellationToken = default) where T : AuditBase;
    IQueryable<T> GetAuditLogsQueryable<T>(Expression<Func<T, bool>>? expression = null) where T : AuditBase;

    IQueryable<T> GetAuditLogsByEntityId<T>(string entityId, string entityName,
        Expression<Func<T, bool>>? expression = null) where T : AuditLog;
}