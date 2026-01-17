using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProjectsWebApp.DataAccsess.Migrations
{
    /// <inheritdoc />
    public partial class stabilitaetTest : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "DisplayOrder2",
                table: "Skills",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.UpdateData(
                table: "Events",
                keyColumn: "Id",
                keyValue: 1,
                column: "Termin",
                value: new DateTime(2026, 1, 24, 0, 0, 0, 0, DateTimeKind.Local));

            migrationBuilder.UpdateData(
                table: "PortalVideo",
                keyColumn: "Id",
                keyValue: 1,
                column: "UploadDate",
                value: new DateTime(2026, 1, 17, 17, 20, 43, 960, DateTimeKind.Local).AddTicks(653));

            migrationBuilder.UpdateData(
                table: "PromptAiSettings",
                keyColumn: "Id",
                keyValue: 1,
                column: "UpdatedAt",
                value: new DateTime(2026, 1, 17, 16, 20, 43, 959, DateTimeKind.Utc).AddTicks(9995));

            migrationBuilder.UpdateData(
                table: "PromptTypeGuidances",
                keyColumn: "Id",
                keyValue: 1,
                column: "UpdatedAt",
                value: new DateTime(2026, 1, 17, 16, 20, 43, 960, DateTimeKind.Utc).AddTicks(29));

            migrationBuilder.UpdateData(
                table: "PromptTypeGuidances",
                keyColumn: "Id",
                keyValue: 2,
                column: "UpdatedAt",
                value: new DateTime(2026, 1, 17, 16, 20, 43, 960, DateTimeKind.Utc).AddTicks(33));

            migrationBuilder.UpdateData(
                table: "PromptTypeGuidances",
                keyColumn: "Id",
                keyValue: 3,
                column: "UpdatedAt",
                value: new DateTime(2026, 1, 17, 16, 20, 43, 960, DateTimeKind.Utc).AddTicks(34));

            migrationBuilder.UpdateData(
                table: "PromptTypeGuidances",
                keyColumn: "Id",
                keyValue: 4,
                column: "UpdatedAt",
                value: new DateTime(2026, 1, 17, 16, 20, 43, 960, DateTimeKind.Utc).AddTicks(35));

            migrationBuilder.UpdateData(
                table: "PromptTypeGuidances",
                keyColumn: "Id",
                keyValue: 5,
                column: "UpdatedAt",
                value: new DateTime(2026, 1, 17, 16, 20, 43, 960, DateTimeKind.Utc).AddTicks(36));

            migrationBuilder.UpdateData(
                table: "PromptTypeGuidances",
                keyColumn: "Id",
                keyValue: 6,
                column: "UpdatedAt",
                value: new DateTime(2026, 1, 17, 16, 20, 43, 960, DateTimeKind.Utc).AddTicks(37));

            migrationBuilder.UpdateData(
                table: "PromptTypeGuidances",
                keyColumn: "Id",
                keyValue: 7,
                column: "UpdatedAt",
                value: new DateTime(2026, 1, 17, 16, 20, 43, 960, DateTimeKind.Utc).AddTicks(38));

            migrationBuilder.UpdateData(
                table: "Skills",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "DisplayOrder2" },
                values: new object[] { new DateTime(2026, 1, 17, 17, 20, 43, 960, DateTimeKind.Local).AddTicks(349), 0 });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DisplayOrder2",
                table: "Skills");

            migrationBuilder.UpdateData(
                table: "Events",
                keyColumn: "Id",
                keyValue: 1,
                column: "Termin",
                value: new DateTime(2026, 1, 9, 0, 0, 0, 0, DateTimeKind.Local));

            migrationBuilder.UpdateData(
                table: "PortalVideo",
                keyColumn: "Id",
                keyValue: 1,
                column: "UploadDate",
                value: new DateTime(2026, 1, 2, 23, 40, 36, 445, DateTimeKind.Local).AddTicks(3473));

            migrationBuilder.UpdateData(
                table: "PromptAiSettings",
                keyColumn: "Id",
                keyValue: 1,
                column: "UpdatedAt",
                value: new DateTime(2026, 1, 2, 22, 40, 36, 445, DateTimeKind.Utc).AddTicks(2833));

            migrationBuilder.UpdateData(
                table: "PromptTypeGuidances",
                keyColumn: "Id",
                keyValue: 1,
                column: "UpdatedAt",
                value: new DateTime(2026, 1, 2, 22, 40, 36, 445, DateTimeKind.Utc).AddTicks(2869));

            migrationBuilder.UpdateData(
                table: "PromptTypeGuidances",
                keyColumn: "Id",
                keyValue: 2,
                column: "UpdatedAt",
                value: new DateTime(2026, 1, 2, 22, 40, 36, 445, DateTimeKind.Utc).AddTicks(2870));

            migrationBuilder.UpdateData(
                table: "PromptTypeGuidances",
                keyColumn: "Id",
                keyValue: 3,
                column: "UpdatedAt",
                value: new DateTime(2026, 1, 2, 22, 40, 36, 445, DateTimeKind.Utc).AddTicks(2870));

            migrationBuilder.UpdateData(
                table: "PromptTypeGuidances",
                keyColumn: "Id",
                keyValue: 4,
                column: "UpdatedAt",
                value: new DateTime(2026, 1, 2, 22, 40, 36, 445, DateTimeKind.Utc).AddTicks(2871));

            migrationBuilder.UpdateData(
                table: "PromptTypeGuidances",
                keyColumn: "Id",
                keyValue: 5,
                column: "UpdatedAt",
                value: new DateTime(2026, 1, 2, 22, 40, 36, 445, DateTimeKind.Utc).AddTicks(2872));

            migrationBuilder.UpdateData(
                table: "PromptTypeGuidances",
                keyColumn: "Id",
                keyValue: 6,
                column: "UpdatedAt",
                value: new DateTime(2026, 1, 2, 22, 40, 36, 445, DateTimeKind.Utc).AddTicks(2873));

            migrationBuilder.UpdateData(
                table: "PromptTypeGuidances",
                keyColumn: "Id",
                keyValue: 7,
                column: "UpdatedAt",
                value: new DateTime(2026, 1, 2, 22, 40, 36, 445, DateTimeKind.Utc).AddTicks(2874));

            migrationBuilder.UpdateData(
                table: "Skills",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2026, 1, 2, 23, 40, 36, 445, DateTimeKind.Local).AddTicks(3143));
        }
    }
}
