using Microsoft.AspNetCore.Http;

namespace AuditSharp.EntityFrameworkCore;

public class AuditSharpOptions
{
    public bool IsIgnoreUnchanged { get; set; } = false;
    public IHttpContextAccessor? AuditLogHttpContextAccessor { get; set; }
}