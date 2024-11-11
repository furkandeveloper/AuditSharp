namespace AuditSharp.Core.Entities;

public class AuditBase
{
    public required string Id { get; set; }

    public DateTime CreationTime { get; set; }
}