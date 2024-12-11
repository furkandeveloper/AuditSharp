using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AuditSharp.PostgreSql.Migrations
{
    /// <inheritdoc />
    public partial class AddAuditLog : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "audit-sharp");

            migrationBuilder.CreateTable(
                name: "AuditLogs",
                schema: "audit-sharp",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    EntityId = table.Column<string>(type: "text", nullable: false),
                    EntityName = table.Column<string>(type: "text", nullable: false),
                    OperationType = table.Column<string>(type: "text", nullable: false),
                    OldValues = table.Column<string>(type: "text", nullable: false),
                    NewValues = table.Column<string>(type: "text", nullable: false),
                    CreationTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AuditLogs", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AuditLogs_EntityId",
                schema: "audit-sharp",
                table: "AuditLogs",
                column: "EntityId");

            migrationBuilder.CreateIndex(
                name: "IX_AuditLogs_EntityId_EntityName",
                schema: "audit-sharp",
                table: "AuditLogs",
                columns: new[] { "EntityId", "EntityName" });

            migrationBuilder.CreateIndex(
                name: "IX_AuditLogs_EntityName",
                schema: "audit-sharp",
                table: "AuditLogs",
                column: "EntityName");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AuditLogs",
                schema: "audit-sharp");
        }
    }
}
