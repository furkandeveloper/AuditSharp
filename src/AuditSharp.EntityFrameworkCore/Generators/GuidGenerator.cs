using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.ValueGeneration;

namespace AuditSharp.EntityFrameworkCore.Generators;

public class GuidGenerator : ValueGenerator
{
    public override bool GeneratesTemporaryValues => false;

    protected override object? NextValue(EntityEntry entry)
    {
        return Guid.NewGuid();
    }
}