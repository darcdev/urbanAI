using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Urban.AI.Infrastructure.Database.Migrations
{
    /// <inheritdoc />
    public partial class AddIncidentsTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "incidents",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    filing_number = table.Column<string>(type: "character varying(15)", maxLength: 15, nullable: false),
                    image_path = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    location_latitude = table.Column<decimal>(type: "numeric(18,8)", precision: 18, scale: 8, nullable: false),
                    location_longitude = table.Column<decimal>(type: "numeric(18,8)", precision: 18, scale: 8, nullable: false),
                    citizen_email = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    additional_comment = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    caption = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    ai_description = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    category = table.Column<string>(type: "text", nullable: true),
                    severity = table.Column<string>(type: "text", nullable: true),
                    status = table.Column<string>(type: "text", nullable: false),
                    priority = table.Column<string>(type: "text", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    attention_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    municipality_id = table.Column<Guid>(type: "uuid", nullable: false),
                    leader_id = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_incidents", x => x.id);
                    table.ForeignKey(
                        name: "fk_incidents_leader_leader_id",
                        column: x => x.leader_id,
                        principalTable: "leaders",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "fk_incidents_municipalities_municipality_id",
                        column: x => x.municipality_id,
                        principalTable: "municipalities",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "ix_incidents_created_at",
                table: "incidents",
                column: "created_at");

            migrationBuilder.CreateIndex(
                name: "ix_incidents_filing_number",
                table: "incidents",
                column: "filing_number",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_incidents_leader_id",
                table: "incidents",
                column: "leader_id");

            migrationBuilder.CreateIndex(
                name: "ix_incidents_municipality_id",
                table: "incidents",
                column: "municipality_id");

            migrationBuilder.CreateIndex(
                name: "ix_incidents_priority",
                table: "incidents",
                column: "priority");

            migrationBuilder.CreateIndex(
                name: "ix_incidents_status",
                table: "incidents",
                column: "status");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "incidents");
        }
    }
}
