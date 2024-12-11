using AuditSharp.Core.Entities;

namespace AuditSharp.EntityFrameworkCore.Context;

public interface IAuditSharpContext
{
    Task InsertAsync<T>(T entity, CancellationToken cancellationToken = default) where T : AuditBase;
}