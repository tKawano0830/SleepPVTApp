using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SleepPvtTracker.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SleepRecords",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Bedtime = table.Column<DateTime>(type: "TEXT", nullable: false),
                    WakeUpTime = table.Column<DateTime>(type: "TEXT", nullable: false),
                    SleepinessLevel = table.Column<int>(type: "INTEGER", nullable: false),
                    PvtStartTime = table.Column<DateTime>(type: "TEXT", nullable: true),
                    PvtEndTime = table.Column<DateTime>(type: "TEXT", nullable: true),
                    PvtTrialsJson = table.Column<string>(type: "TEXT", nullable: true),
                    PvtExtraFalseStarts = table.Column<int>(type: "INTEGER", nullable: true),
                    Comments = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SleepRecords", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SleepRecords");
        }
    }
}
