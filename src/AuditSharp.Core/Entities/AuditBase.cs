namespace AuditSharp.Core.Entities;

public class AuditBase
{
    public Guid Id { get; set; }

    public DateTime CreationTime { get; set; }
}