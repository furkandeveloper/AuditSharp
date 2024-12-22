using Microsoft.AspNetCore.Http;

namespace AuditSharp.EntityFrameworkCore;

public class AuditSharpOptions
{
    public bool IsIgnoreUnchanged { get; set; } = false;
}