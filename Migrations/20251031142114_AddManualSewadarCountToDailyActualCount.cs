using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace bhati_jatha_count_report.Migrations
{
    /// <inheritdoc />
    public partial class AddManualSewadarCountToDailyActualCount : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ManualSewadarCount",
                table: "DailyActualCounts",
                type: "integer",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ManualSewadarCount",
                table: "DailyActualCounts");
        }
    }
}
