using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace bhati_jatha_count_report.Migrations
{
    /// <inheritdoc />
    public partial class AddSewaNominalRoll : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SewaNominalRolls",
                columns: table => new
                {
                    NominalRollToken = table.Column<string>(type: "TEXT", nullable: false),
                    SewaDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    RowId = table.Column<int>(type: "INTEGER", nullable: false),
                    SewaName = table.Column<string>(type: "TEXT", nullable: false),
                    Department = table.Column<string>(type: "TEXT", nullable: false),
                    Zone = table.Column<string>(type: "TEXT", nullable: false),
                    CentreName = table.Column<string>(type: "TEXT", nullable: false),
                    JourneyDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    SewaDuration = table.Column<int>(type: "INTEGER", nullable: false),
                    Males = table.Column<int>(type: "INTEGER", nullable: false),
                    Females = table.Column<int>(type: "INTEGER", nullable: false),
                    TotalSewadars = table.Column<int>(type: "INTEGER", nullable: false),
                    SewaType = table.Column<string>(type: "TEXT", nullable: false),
                    SewaStartTime = table.Column<string>(type: "TEXT", nullable: false),
                    InchargeName = table.Column<string>(type: "TEXT", nullable: false),
                    InchargeContact = table.Column<string>(type: "TEXT", nullable: false),
                    Remarks = table.Column<string>(type: "TEXT", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SewaNominalRolls", x => new { x.NominalRollToken, x.SewaDate });
                });

            migrationBuilder.CreateIndex(
                name: "IX_SewaNominalRolls_RowId",
                table: "SewaNominalRolls",
                column: "RowId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SewaNominalRolls");
        }
    }
}
