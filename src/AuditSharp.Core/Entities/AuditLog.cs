namespace AuditSharp.Core.Entities;

public class AuditLog(string entityName, string operationType, string oldValues, string newValues, string entityId) : AuditBase
{
    public string EntityId { get; private set; } = entityId;
    public string EntityName { get; private set; } = entityName;
    public string OperationType { get; private set; } = operationType;
    public string OldValues { get; private set; } = oldValues;
    public string NewValues { get; private set; } = newValues;
}