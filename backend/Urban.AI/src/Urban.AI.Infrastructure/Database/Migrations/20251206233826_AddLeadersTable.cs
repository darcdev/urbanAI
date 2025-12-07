using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Urban.AI.Infrastructure.Database.Migrations
{
    /// <inheritdoc />
    public partial class AddLeadersTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "leaders",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    department_id = table.Column<Guid>(type: "uuid", nullable: false),
                    municipality_id = table.Column<Guid>(type: "uuid", nullable: false),
                    latitude = table.Column<decimal>(type: "numeric(18,8)", precision: 18, scale: 8, nullable: false),
                    longitude = table.Column<decimal>(type: "numeric(18,8)", precision: 18, scale: 8, nullable: false),
                    is_active = table.Column<bool>(type: "boolean", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_leaders", x => x.id);
                    table.ForeignKey(
                        name: "fk_leaders_departments_department_id",
                        column: x => x.department_id,
                        principalTable: "departments",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_leaders_municipalities_municipality_id",
                        column: x => x.municipality_id,
                        principalTable: "municipalities",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_leaders_user_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "ix_leaders_department_id",
                table: "leaders",
                column: "department_id");

            migrationBuilder.CreateIndex(
                name: "ix_leaders_is_active",
                table: "leaders",
                column: "is_active");

            migrationBuilder.CreateIndex(
                name: "ix_leaders_municipality_id",
                table: "leaders",
                column: "municipality_id");

            migrationBuilder.CreateIndex(
                name: "ix_leaders_user_id",
                table: "leaders",
                column: "user_id",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "leaders");
        }
    }
}
