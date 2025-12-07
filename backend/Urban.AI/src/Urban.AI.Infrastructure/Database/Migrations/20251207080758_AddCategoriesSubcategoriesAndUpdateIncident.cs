using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Urban.AI.Infrastructure.Database.Migrations
{
    /// <inheritdoc />
    public partial class AddCategoriesSubcategoriesAndUpdateIncident : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "caption",
                table: "incidents");

            migrationBuilder.DropColumn(
                name: "category",
                table: "incidents");

            migrationBuilder.DropColumn(
                name: "severity",
                table: "incidents");

            migrationBuilder.AddColumn<Guid>(
                name: "category_id",
                table: "incidents",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "subcategory_id",
                table: "incidents",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "categories",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    code = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_categories", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "subcategories",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: false),
                    description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    category_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_subcategories", x => x.id);
                    table.ForeignKey(
                        name: "fk_subcategories_categories_category_id",
                        column: x => x.category_id,
                        principalTable: "categories",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_incidents_category_id",
                table: "incidents",
                column: "category_id");

            migrationBuilder.CreateIndex(
                name: "ix_incidents_subcategory_id",
                table: "incidents",
                column: "subcategory_id");

            migrationBuilder.CreateIndex(
                name: "ix_categories_code",
                table: "categories",
                column: "code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_subcategories_category_id",
                table: "subcategories",
                column: "category_id");

            migrationBuilder.CreateIndex(
                name: "ix_subcategories_category_id_name",
                table: "subcategories",
                columns: new[] { "category_id", "name" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "fk_incidents_categories_category_id",
                table: "incidents",
                column: "category_id",
                principalTable: "categories",
                principalColumn: "id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "fk_incidents_subcategory_subcategory_id",
                table: "incidents",
                column: "subcategory_id",
                principalTable: "subcategories",
                principalColumn: "id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_incidents_categories_category_id",
                table: "incidents");

            migrationBuilder.DropForeignKey(
                name: "fk_incidents_subcategory_subcategory_id",
                table: "incidents");

            migrationBuilder.DropTable(
                name: "subcategories");

            migrationBuilder.DropTable(
                name: "categories");

            migrationBuilder.DropIndex(
                name: "ix_incidents_category_id",
                table: "incidents");

            migrationBuilder.DropIndex(
                name: "ix_incidents_subcategory_id",
                table: "incidents");

            migrationBuilder.DropColumn(
                name: "category_id",
                table: "incidents");

            migrationBuilder.DropColumn(
                name: "subcategory_id",
                table: "incidents");

            migrationBuilder.AddColumn<string>(
                name: "caption",
                table: "incidents",
                type: "character varying(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "category",
                table: "incidents",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "severity",
                table: "incidents",
                type: "text",
                nullable: true);
        }
    }
}
