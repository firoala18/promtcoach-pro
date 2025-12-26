using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace ProjectsWebApp.DataAccsess.Migrations
{
    /// <inheritdoc />
    public partial class AddUserActivityDailyTracking : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "UserDailyActivityStats",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<string>(type: "text", nullable: false),
                    Group = table.Column<string>(type: "text", nullable: true),
                    DateUtc = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    TotalEvents = table.Column<int>(type: "integer", nullable: false),
                    LoginCount = table.Column<int>(type: "integer", nullable: false),
                    PromptGenerateCount = table.Column<int>(type: "integer", nullable: false),
                    FilterGenerateCount = table.Column<int>(type: "integer", nullable: false),
                    SmartSelectionCount = table.Column<int>(type: "integer", nullable: false),
                    AssistantChatCount = table.Column<int>(type: "integer", nullable: false),
                    AssistantChatStreamCount = table.Column<int>(type: "integer", nullable: false),
                    PromptSaveCollectionCount = table.Column<int>(type: "integer", nullable: false),
                    PromptPublishLibraryCount = table.Column<int>(type: "integer", nullable: false),
                    TotalDurationSeconds = table.Column<int>(type: "integer", nullable: false),
                    LastUpdatedAtUtc = table.Column<DateTime>(type: "timestamp without time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserDailyActivityStats", x => x.Id);
                });

            migrationBuilder.UpdateData(
                table: "PortalVideo",
                keyColumn: "Id",
                keyValue: 1,
                column: "UploadDate",
                value: new DateTime(2025, 11, 28, 8, 55, 29, 311, DateTimeKind.Local).AddTicks(8651));

            migrationBuilder.UpdateData(
                table: "PromptAiSettings",
                keyColumn: "Id",
                keyValue: 1,
                column: "UpdatedAt",
                value: new DateTime(2025, 11, 28, 7, 55, 29, 311, DateTimeKind.Utc).AddTicks(8103));

            migrationBuilder.UpdateData(
                table: "PromptTypeGuidances",
                keyColumn: "Id",
                keyValue: 1,
                column: "UpdatedAt",
                value: new DateTime(2025, 11, 28, 7, 55, 29, 311, DateTimeKind.Utc).AddTicks(8128));

            migrationBuilder.UpdateData(
                table: "PromptTypeGuidances",
                keyColumn: "Id",
                keyValue: 2,
                column: "UpdatedAt",
                value: new DateTime(2025, 11, 28, 7, 55, 29, 311, DateTimeKind.Utc).AddTicks(8129));

            migrationBuilder.UpdateData(
                table: "PromptTypeGuidances",
                keyColumn: "Id",
                keyValue: 3,
                column: "UpdatedAt",
                value: new DateTime(2025, 11, 28, 7, 55, 29, 311, DateTimeKind.Utc).AddTicks(8130));

            migrationBuilder.UpdateData(
                table: "PromptTypeGuidances",
                keyColumn: "Id",
                keyValue: 4,
                column: "UpdatedAt",
                value: new DateTime(2025, 11, 28, 7, 55, 29, 311, DateTimeKind.Utc).AddTicks(8130));

            migrationBuilder.UpdateData(
                table: "PromptTypeGuidances",
                keyColumn: "Id",
                keyValue: 5,
                column: "UpdatedAt",
                value: new DateTime(2025, 11, 28, 7, 55, 29, 311, DateTimeKind.Utc).AddTicks(8131));

            migrationBuilder.UpdateData(
                table: "PromptTypeGuidances",
                keyColumn: "Id",
                keyValue: 6,
                column: "UpdatedAt",
                value: new DateTime(2025, 11, 28, 7, 55, 29, 311, DateTimeKind.Utc).AddTicks(8132));

            migrationBuilder.UpdateData(
                table: "PromptTypeGuidances",
                keyColumn: "Id",
                keyValue: 7,
                column: "UpdatedAt",
                value: new DateTime(2025, 11, 28, 7, 55, 29, 311, DateTimeKind.Utc).AddTicks(8132));

            migrationBuilder.UpdateData(
                table: "Skills",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 28, 8, 55, 29, 311, DateTimeKind.Local).AddTicks(8347));

            migrationBuilder.CreateIndex(
                name: "IX_UserDailyActivityStats_UserId_Group_DateUtc",
                table: "UserDailyActivityStats",
                columns: new[] { "UserId", "Group", "DateUtc" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserDailyActivityStats");

            migrationBuilder.UpdateData(
                table: "PortalVideo",
                keyColumn: "Id",
                keyValue: 1,
                column: "UploadDate",
                value: new DateTime(2025, 11, 28, 8, 48, 58, 562, DateTimeKind.Local).AddTicks(2567));

            migrationBuilder.UpdateData(
                table: "PromptAiSettings",
                keyColumn: "Id",
                keyValue: 1,
                column: "UpdatedAt",
                value: new DateTime(2025, 11, 28, 7, 48, 58, 562, DateTimeKind.Utc).AddTicks(1782));

            migrationBuilder.UpdateData(
                table: "PromptTypeGuidances",
                keyColumn: "Id",
                keyValue: 1,
                column: "UpdatedAt",
                value: new DateTime(2025, 11, 28, 7, 48, 58, 562, DateTimeKind.Utc).AddTicks(1813));

            migrationBuilder.UpdateData(
                table: "PromptTypeGuidances",
                keyColumn: "Id",
                keyValue: 2,
                column: "UpdatedAt",
                value: new DateTime(2025, 11, 28, 7, 48, 58, 562, DateTimeKind.Utc).AddTicks(1814));

            migrationBuilder.UpdateData(
                table: "PromptTypeGuidances",
                keyColumn: "Id",
                keyValue: 3,
                column: "UpdatedAt",
                value: new DateTime(2025, 11, 28, 7, 48, 58, 562, DateTimeKind.Utc).AddTicks(1815));

            migrationBuilder.UpdateData(
                table: "PromptTypeGuidances",
                keyColumn: "Id",
                keyValue: 4,
                column: "UpdatedAt",
                value: new DateTime(2025, 11, 28, 7, 48, 58, 562, DateTimeKind.Utc).AddTicks(1816));

            migrationBuilder.UpdateData(
                table: "PromptTypeGuidances",
                keyColumn: "Id",
                keyValue: 5,
                column: "UpdatedAt",
                value: new DateTime(2025, 11, 28, 7, 48, 58, 562, DateTimeKind.Utc).AddTicks(1817));

            migrationBuilder.UpdateData(
                table: "PromptTypeGuidances",
                keyColumn: "Id",
                keyValue: 6,
                column: "UpdatedAt",
                value: new DateTime(2025, 11, 28, 7, 48, 58, 562, DateTimeKind.Utc).AddTicks(1817));

            migrationBuilder.UpdateData(
                table: "PromptTypeGuidances",
                keyColumn: "Id",
                keyValue: 7,
                column: "UpdatedAt",
                value: new DateTime(2025, 11, 28, 7, 48, 58, 562, DateTimeKind.Utc).AddTicks(1818));

            migrationBuilder.UpdateData(
                table: "Skills",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 28, 8, 48, 58, 562, DateTimeKind.Local).AddTicks(2190));
        }
    }
}
