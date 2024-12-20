using AuditSharp.Core.Abstracts;

namespace AuditSharp.Sample.Entities;

public class Person : IAuditable
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Surname { get; set; }
    public DateTime BirthDate { get; set; }
    public string Description { get; set; }
}