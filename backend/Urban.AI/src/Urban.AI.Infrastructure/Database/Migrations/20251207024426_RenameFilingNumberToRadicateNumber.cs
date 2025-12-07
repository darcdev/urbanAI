using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Urban.AI.Infrastructure.Database.Migrations
{
    /// <inheritdoc />
    public partial class RenameFilingNumberToRadicateNumber : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "ix_incidents_filing_number",
                table: "incidents");

            migrationBuilder.DropColumn(
                name: "filing_number",
                table: "incidents");

            migrationBuilder.AddColumn<string>(
                name: "radicate_number",
                table: "incidents",
                type: "character varying(15)",
                maxLength: 15,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "ix_incidents_radicate_number",
                table: "incidents",
                column: "radicate_number",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "ix_incidents_radicate_number",
                table: "incidents");

            migrationBuilder.DropColumn(
                name: "radicate_number",
                table: "incidents");

            migrationBuilder.AddColumn<string>(
                name: "filing_number",
                table: "incidents",
                type: "character varying(25)",
                maxLength: 25,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "ix_incidents_filing_number",
                table: "incidents",
                column: "filing_number",
                unique: true);
        }
    }
}
