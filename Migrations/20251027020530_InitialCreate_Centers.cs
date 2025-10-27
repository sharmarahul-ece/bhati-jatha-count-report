using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace bhati_jatha_count_report.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate_Centers : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Centers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    CenterName = table.Column<string>(type: "TEXT", nullable: false),
                    CenterType = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Centers", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Centers");
        }
    }
}
