using AuditSharp.Core.Entities;
using AuditSharp.EntityFrameworkCore.Context;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AuditSharp.Sample.Controllers
{
    /// <summary>
    /// Controller for handling audit log operations.
    /// </summary>
    public class AuditLogController : Controller
    {
        private readonly IAuditSharpContext _auditSharpContext;

        /// <summary>
        /// Initializes a new instance of the <see cref="AuditLogController"/> class.
        /// </summary>
        /// <param name="auditSharpContext">The context for accessing audit logs.</param>
        public AuditLogController(IAuditSharpContext auditSharpContext)
        {
            _auditSharpContext = auditSharpContext;
        }

        /// <summary>
        /// Gets all audit logs.
        /// </summary>
        /// <returns>A list of audit logs.</returns>
        [HttpGet("GetAuditLogs")]
        public async Task<List<AuditLog>> GetAsync()
        {
            return await _auditSharpContext.GetAuditLogsQueryable<AuditLog>().ToListAsync();
        }

        /// <summary>
        /// Gets audit logs by entity ID and entity name.
        /// </summary>
        /// <param name="entityId">The ID of the entity.</param>
        /// <param name="entityName">The name of the entity.</param>
        /// <returns>A list of audit logs for the specified entity.</returns>
        [HttpGet("GetAuditLogsByEntityId/{entityId}/{entityName}")]
        public async Task<List<AuditLog>> GetByEntityIdAsync(string entityId, string entityName)
        {
            return await _auditSharpContext.GetAuditLogsByEntityId<AuditLog>(entityId, entityName).ToListAsync();
        }
    }
}