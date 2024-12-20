#define postgresql // mongodb, postgresql

#if mongodb
using AuditSharp.MongoDb.Extensions;
using MongoDB.Driver;
#endif

using AuditSharp.EntityFrameworkCore;
using AuditSharp.Sample.DataAccess;
using Microsoft.EntityFrameworkCore;
#if postgresql
using AuditSharp.PostgreSql.Extensions;
#endif

namespace AuditSharp.Sample;

public class Startup
{
    private readonly IConfiguration _configuration;

    public Startup(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public void ConfigureServices(IServiceCollection services)
    {
        services.AddControllers();
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();
        services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
        services.AddDbContext<ExampleDbContext>(options =>
        {
            // TODO: HttpContextAccessor farkli bir yontemle alinabilir.
            var httpContextAccessor = services.BuildServiceProvider().GetService<IHttpContextAccessor>();
            options.UseInMemoryDatabase("InMemoryDb")
                .RegisterAuditSharp(new AuditSharpOptions
                    { IsIgnoreUnchanged = true, AuditLogHttpContextAccessor = httpContextAccessor });
        });

        services.AddAuditSharp(options =>
        {
#if mongodb
            var mongoClient = new MongoClient("{Your Connection String}");
            options.UseMongoDB(mongoClient, "AuditLog");
#endif
#if postgresql
            options.UseNpgsql("{Your Connection String}");
#endif
        });
    }

    public void Configure(WebApplication app, IWebHostEnvironment env)
    {
        app.UseAuditSharp();
        app.UseSwagger();
        app.UseSwaggerUI();

        app.UseHttpsRedirection();
        app.UseAuthorization();
        app.MapControllers();
    }
}