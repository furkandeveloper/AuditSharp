using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using MongoDB.Driver;

namespace AuditSharp.MongoDb.Context;

public class DesignTimeDbContext : IDesignTimeDbContextFactory<AuditSharpMongoDbContext>
{
    public AuditSharpMongoDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<AuditSharpMongoDbContext>();
        var mongoClient = new MongoClient("{Your Connection String}");
        optionsBuilder.UseMongoDB(mongoClient, "{Your Database Name}");
        return new AuditSharpMongoDbContext(optionsBuilder.Options);
    }
}