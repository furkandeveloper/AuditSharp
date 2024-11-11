using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.ValueGeneration;

namespace AuditSharp.EntityFrameworkCore.Generators;

public class DateGenerator : ValueGenerator
{
    protected override object? NextValue(EntityEntry entry)
    {
        return DateTime.UtcNow;
    }

    public override bool GeneratesTemporaryValues => false;
}