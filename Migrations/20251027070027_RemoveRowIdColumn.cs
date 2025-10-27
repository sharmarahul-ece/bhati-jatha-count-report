using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace bhati_jatha_count_report.Migrations
{
    /// <inheritdoc />
    public partial class RemoveRowIdColumn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_SewaNominalRolls_RowId",
                table: "SewaNominalRolls");

            migrationBuilder.DropColumn(
                name: "RowId",
                table: "SewaNominalRolls");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "RowId",
                table: "SewaNominalRolls",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_SewaNominalRolls_RowId",
                table: "SewaNominalRolls",
                column: "RowId",
                unique: true);
        }
    }
}
