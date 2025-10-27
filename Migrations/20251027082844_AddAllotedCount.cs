using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace bhati_jatha_count_report.Migrations
{
    /// <inheritdoc />
    public partial class AddAllotedCount : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AllotedCounts",
                columns: table => new
                {
                    WeekDay = table.Column<int>(type: "INTEGER", nullable: false),
                    CenterId = table.Column<int>(type: "INTEGER", nullable: false),
                    SewaTypeId = table.Column<int>(type: "INTEGER", nullable: false),
                    Count = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AllotedCounts", x => new { x.WeekDay, x.CenterId, x.SewaTypeId });
                    table.ForeignKey(
                        name: "FK_AllotedCounts_Centers_CenterId",
                        column: x => x.CenterId,
                        principalTable: "Centers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AllotedCounts_SewaTypes_SewaTypeId",
                        column: x => x.SewaTypeId,
                        principalTable: "SewaTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AllotedCounts_CenterId",
                table: "AllotedCounts",
                column: "CenterId");

            migrationBuilder.CreateIndex(
                name: "IX_AllotedCounts_SewaTypeId",
                table: "AllotedCounts",
                column: "SewaTypeId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AllotedCounts");
        }
    }
}
