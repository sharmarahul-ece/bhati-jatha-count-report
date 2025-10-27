using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace bhati_jatha_count_report.Migrations
{
    /// <inheritdoc />
    public partial class RemoveTokenNumberFromDailyActualCount : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TokenNumber",
                table: "DailyActualCounts");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "TokenNumber",
                table: "DailyActualCounts",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);
        }
    }
}
