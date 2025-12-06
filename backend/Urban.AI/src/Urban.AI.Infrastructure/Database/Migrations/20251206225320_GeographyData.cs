using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Urban.AI.Infrastructure.Database.Migrations
{
    /// <inheritdoc />
    public partial class GeographyData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "departments",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    department_dane_code = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    latitude = table.Column<decimal>(type: "numeric(18,8)", precision: 18, scale: 8, nullable: true),
                    longitude = table.Column<decimal>(type: "numeric(18,8)", precision: 18, scale: 8, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_departments", x => x.id);
                    table.UniqueConstraint("ak_departments_department_dane_code", x => x.department_dane_code);
                });

            migrationBuilder.CreateTable(
                name: "municipalities",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    municipality_dane_code = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    department_dane_code = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    latitude = table.Column<decimal>(type: "numeric(18,8)", precision: 18, scale: 8, nullable: true),
                    longitude = table.Column<decimal>(type: "numeric(18,8)", precision: 18, scale: 8, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_municipalities", x => x.id);
                    table.UniqueConstraint("ak_municipalities_municipality_dane_code", x => x.municipality_dane_code);
                    table.ForeignKey(
                        name: "fk_municipalities_departments_department_dane_code",
                        column: x => x.department_dane_code,
                        principalTable: "departments",
                        principalColumn: "department_dane_code",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "townships",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    township_dane_code = table.Column<string>(type: "character varying(15)", maxLength: 15, nullable: false),
                    name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    municipality_dane_code = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_townships", x => x.id);
                    table.ForeignKey(
                        name: "fk_townships_municipalities_municipality_dane_code",
                        column: x => x.municipality_dane_code,
                        principalTable: "municipalities",
                        principalColumn: "municipality_dane_code",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "ix_departments_department_dane_code",
                table: "departments",
                column: "department_dane_code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_departments_name",
                table: "departments",
                column: "name");

            migrationBuilder.CreateIndex(
                name: "ix_municipalities_department_dane_code",
                table: "municipalities",
                column: "department_dane_code");

            migrationBuilder.CreateIndex(
                name: "ix_municipalities_municipality_dane_code",
                table: "municipalities",
                column: "municipality_dane_code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_municipalities_name",
                table: "municipalities",
                column: "name");

            migrationBuilder.CreateIndex(
                name: "ix_townships_municipality_dane_code",
                table: "townships",
                column: "municipality_dane_code");

            migrationBuilder.CreateIndex(
                name: "ix_townships_name",
                table: "townships",
                column: "name");

            migrationBuilder.CreateIndex(
                name: "ix_townships_township_dane_code",
                table: "townships",
                column: "township_dane_code",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "townships");

            migrationBuilder.DropTable(
                name: "municipalities");

            migrationBuilder.DropTable(
                name: "departments");
        }
    }
}
