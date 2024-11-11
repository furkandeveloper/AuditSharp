using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.ValueGeneration;

namespace AuditSharp.EntityFrameworkCore.Generators;

public class GuidGenerator : ValueGenerator
{
    protected override object? NextValue(EntityEntry entry)
    {
        return Guid.NewGuid();
    }

    public override bool GeneratesTemporaryValues => false;
}