## How To Use AuditSharp

AuditSharp is a lightweight library for auditing operations in .NET applications. It simplifies tracking changes, recording metadata, and storing audit trails in your application. This document provides step-by-step guidance on how to integrate and use AuditSharp in your project.

### Installation

To get started, install AuditSharh vi NuGet:

<details>
<summary>
For PostgreSQL

</summary>

```
dotnet add package AuditSharp.PostgreSql
```

### Configuration

```csharp
var host = builder.Configuration.GetValue<string>("{YOUR_CONNECTION_STRINĞ}");
builder.Services.AddDbContext<YourDbContext>(options => options.UseNpgsql(host).RegisterAuditSharp());
builder.Services.AddAuditSharp(options =>
{
    options.UseNpgsql(host);
});
```

```csharp
app.UseAuditSharp();
```
</details>


<details>
<summary>
For SqlServer

</summary>


```
dotnet add package AuditSharp.SqlServer
```

### Configuration

```csharp
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ??
throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseSqlServer(connectionString);
    options.RegisterAuditSharp();
});
builder.Services.AddAuditSharp(optionsBuilder => optionsBuilder.UseSqlServer(connectionString));
```

```csharp
app.UseAuditSharp();
```

</details>


Everything is ready now. ✅

When your application runs, AuditSharp automatically accesses your database and applies all necessary migrations.

Audit Sharp tracks all SaveChanges() steps and saves them in its table within the auditor-sharp schema.

You can check the table to track changes.

The logs recorded in the sample Add, Update and Delete operations are as follows;

<img width="1176" alt="Screenshot 2024-12-12 at 15 14 40" src="https://github.com/user-attachments/assets/9b4a980b-1021-4c33-b750-6db13cc36e7d" />

------

AuditSharp is just taking its first steps. 🐣

Some features considered for the future are as follows;

- [ ] Advanced querying on AuditLog
- [ ] Ignore any entity or field
- [ ] Log Dashboard
- [ ] Detection of changing areas on AuditLog

and more... 🚀
