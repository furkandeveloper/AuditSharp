using AuditSharp.Sample.Entities;
using Microsoft.EntityFrameworkCore;

namespace AuditSharp.Sample.DataAccess;

public class ExampleDbContext : DbContext
{
    public ExampleDbContext(DbContextOptions<ExampleDbContext> options)
        : base(options)
    {
    }

    public DbSet<Person> Persons { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseInMemoryDatabase("ExampleDb");
    }
}