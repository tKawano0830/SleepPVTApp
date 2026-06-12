using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

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
                    PvtExtraFalseStarts = table.Column<int>(type: "INTEGER", nullable: true),
                    Comments = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SleepRecords", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SleepRecordPvtTrials",
                columns: table => new
                {
                    PvtResultSleepRecordId = table.Column<Guid>(type: "TEXT", nullable: false),
                    Id = table.Column<int>(type: "INTEGER", nullable: false),
                    ChacngedAt = table.Column<long>(type: "INTEGER", nullable: false),
                    ClickedAt = table.Column<long>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SleepRecordPvtTrials", x => new { x.PvtResultSleepRecordId, x.Id });
                    table.ForeignKey(
                        name: "FK_SleepRecordPvtTrials_SleepRecords_PvtResultSleepRecordId",
                        column: x => x.PvtResultSleepRecordId,
                        principalTable: "SleepRecords",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "SleepRecords",
                columns: new[] { "Id", "Bedtime", "Comments", "WakeUpTime", "SleepinessLevel" },
                values: new object[,]
                {
                    { new Guid("11111111-1111-1111-1111-111111111111"), new DateTime(2026, 6, 10, 23, 0, 0, 0, DateTimeKind.Unspecified), "よく眠れた", new DateTime(2026, 6, 11, 7, 0, 0, 0, DateTimeKind.Unspecified), 1 },
                    { new Guid("22222222-2222-2222-2222-222222222222"), new DateTime(2026, 6, 12, 1, 0, 0, 0, DateTimeKind.Unspecified), "", new DateTime(2026, 6, 12, 7, 0, 0, 0, DateTimeKind.Unspecified), 7 }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SleepRecordPvtTrials");

            migrationBuilder.DropTable(
                name: "SleepRecords");
        }
    }
}
