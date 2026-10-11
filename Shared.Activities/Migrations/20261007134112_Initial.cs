using System;
using System.Net;
using System.Text.Json;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Shared.Activities.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "activities");

            migrationBuilder.CreateTable(
                name: "user_activities",
                schema: "activities",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    trace_id = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: true),
                    tenant_id = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    actor_id = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    actor_type = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    owner_id = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    impersonator_id = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    execution_type = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    verb = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    object_id = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    object_type = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    target_id = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    target_type = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    metadata = table.Column<JsonDocument>(type: "jsonb", nullable: true),
                    client_ip = table.Column<IPAddress>(type: "inet", nullable: true),
                    user_agent = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: true),
                    device = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    country = table.Column<string>(type: "character varying(5)", maxLength: 5, nullable: true),
                    client_type = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: true),
                    occurred_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_user_activities", x => x.id);
                });

            migrationBuilder.CreateIndex(
                name: "idx_user_activities_actor_occurred",
                schema: "activities",
                table: "user_activities",
                columns: new[] { "actor_id", "actor_type", "occurred_at" },
                filter: "actor_id IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "idx_user_activities_ip_occurred",
                schema: "activities",
                table: "user_activities",
                columns: new[] { "client_ip", "occurred_at" },
                filter: "client_ip IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "idx_user_activities_owner_occurred",
                schema: "activities",
                table: "user_activities",
                columns: new[] { "owner_id", "occurred_at" },
                filter: "owner_id IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "idx_user_activities_trace_id",
                schema: "activities",
                table: "user_activities",
                column: "trace_id",
                filter: "trace_id IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "idx_user_activities_verb_occurred",
                schema: "activities",
                table: "user_activities",
                columns: new[] { "verb", "occurred_at" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "user_activities",
                schema: "activities");
        }
    }
}
