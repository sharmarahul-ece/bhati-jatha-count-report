using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace bhati_jatha_count_report.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Centers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CenterName = table.Column<string>(type: "text", nullable: false),
                    CenterType = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Centers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SewaNominalRolls",
                columns: table => new
                {
                    NominalRollToken = table.Column<string>(type: "text", nullable: false),
                    SewaDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    SewaName = table.Column<string>(type: "text", nullable: false),
                    Department = table.Column<string>(type: "text", nullable: false),
                    Zone = table.Column<string>(type: "text", nullable: false),
                    CentreName = table.Column<string>(type: "text", nullable: false),
                    JourneyDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    SewaDuration = table.Column<int>(type: "integer", nullable: false),
                    Males = table.Column<int>(type: "integer", nullable: false),
                    Females = table.Column<int>(type: "integer", nullable: false),
                    TotalSewadars = table.Column<int>(type: "integer", nullable: false),
                    SewaType = table.Column<string>(type: "text", nullable: false),
                    SewaStartTime = table.Column<string>(type: "text", nullable: false),
                    InchargeName = table.Column<string>(type: "text", nullable: false),
                    InchargeContact = table.Column<string>(type: "text", nullable: false),
                    Remarks = table.Column<string>(type: "text", nullable: false),
                    SourceFileName = table.Column<string>(type: "text", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SewaNominalRolls", x => new { x.NominalRollToken, x.SewaDate });
                });

            migrationBuilder.CreateTable(
                name: "SewaTypes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    SewaName = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SewaTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AllotedCounts",
                columns: table => new
                {
                    WeekDay = table.Column<int>(type: "integer", nullable: false),
                    CenterId = table.Column<int>(type: "integer", nullable: false),
                    SewaTypeId = table.Column<int>(type: "integer", nullable: false),
                    Count = table.Column<int>(type: "integer", nullable: false)
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

            migrationBuilder.CreateTable(
                name: "DailyActualCounts",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Date = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    CenterId = table.Column<int>(type: "integer", nullable: false),
                    SewaTypeId = table.Column<int>(type: "integer", nullable: false),
                    Count = table.Column<int>(type: "integer", nullable: false),
                    NominalRollToken = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false)
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
                name: "IX_AllotedCounts_CenterId",
                table: "AllotedCounts",
                column: "CenterId");

            migrationBuilder.CreateIndex(
                name: "IX_AllotedCounts_SewaTypeId",
                table: "AllotedCounts",
                column: "SewaTypeId");

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
                name: "AllotedCounts");

            migrationBuilder.DropTable(
                name: "DailyActualCounts");

            migrationBuilder.DropTable(
                name: "SewaNominalRolls");

            migrationBuilder.DropTable(
                name: "Centers");

            migrationBuilder.DropTable(
                name: "SewaTypes");
        }
    }
}
