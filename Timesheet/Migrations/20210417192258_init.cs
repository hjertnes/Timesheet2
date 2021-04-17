using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Timesheet.Migrations
{
    public partial class init : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Entries",
                columns: table => new
                {
                    EntryId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Start = table.Column<DateTime>(type: "TEXT", nullable: false),
                    End = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Excluded = table.Column<bool>(type: "INTEGER", nullable: false),
                    Off = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Entries", x => x.EntryId);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Entries");
        }
    }
}
