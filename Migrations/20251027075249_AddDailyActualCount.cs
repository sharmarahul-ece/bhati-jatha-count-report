using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace bhati_jatha_count_report.Migrations
{
    /// <inheritdoc />
    public partial class AddDailyActualCount : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DailyActualCounts",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Date = table.Column<DateTime>(type: "TEXT", nullable: false),
                    TokenNumber = table.Column<int>(type: "INTEGER", nullable: false),
                    CenterId = table.Column<int>(type: "INTEGER", nullable: false),
                    SewaTypeId = table.Column<int>(type: "INTEGER", nullable: false),
                    Count = table.Column<int>(type: "INTEGER", nullable: false),
                    NominalRollToken = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DailyActualCounts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DailyActualCounts_Centers_CenterId",
                        column: x => x.CenterId,
                        principalTable: "Centers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DailyActualCounts_SewaTypes_SewaTypeId",
                        column: x => x.SewaTypeId,
                        principalTable: "SewaTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DailyActualCounts_CenterId",
                table: "DailyActualCounts",
                column: "CenterId");

            migrationBuilder.CreateIndex(
                name: "IX_DailyActualCounts_SewaTypeId",
                table: "DailyActualCounts",
                column: "SewaTypeId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DailyActualCounts");
        }
    }
}
