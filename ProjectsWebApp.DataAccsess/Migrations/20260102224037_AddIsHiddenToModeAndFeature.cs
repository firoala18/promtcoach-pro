using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProjectsWebApp.DataAccsess.Migrations
{
    /// <inheritdoc />
    public partial class AddIsHiddenToModeAndFeature : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsHidden",
                table: "Modes",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsHidden",
                table: "Features",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.UpdateData(
                table: "Events",
                keyColumn: "Id",
                keyValue: 1,
                column: "Termin",
                value: new DateTime(2026, 1, 9, 0, 0, 0, 0, DateTimeKind.Local));

            migrationBuilder.UpdateData(
                table: "Features",
                keyColumn: "Id",
                keyValue: 1,
                column: "IsHidden",
                value: false);

            migrationBuilder.UpdateData(
                table: "Features",
                keyColumn: "Id",
                keyValue: 2,
                column: "IsHidden",
                value: false);

            migrationBuilder.UpdateData(
                table: "Features",
                keyColumn: "Id",
                keyValue: 3,
                column: "IsHidden",
                value: false);

            migrationBuilder.UpdateData(
                table: "Modes",
                keyColumn: "Id",
                keyValue: 1,
                column: "IsHidden",
                value: false);

            migrationBuilder.UpdateData(
                table: "Modes",
                keyColumn: "Id",
                keyValue: 2,
                column: "IsHidden",
                value: false);

            migrationBuilder.UpdateData(
                table: "Modes",
                keyColumn: "Id",
                keyValue: 3,
                column: "IsHidden",
                value: false);

            migrationBuilder.UpdateData(
                table: "Modes",
                keyColumn: "Id",
                keyValue: 4,
                column: "IsHidden",
                value: false);

            migrationBuilder.UpdateData(
                table: "Modes",
                keyColumn: "Id",
                keyValue: 5,
                column: "IsHidden",
                value: false);

            migrationBuilder.UpdateData(
                table: "Modes",
                keyColumn: "Id",
                keyValue: 6,
                column: "IsHidden",
                value: false);

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsHidden",
                table: "Modes");

            migrationBuilder.DropColumn(
                name: "IsHidden",
                table: "Features");

            migrationBuilder.UpdateData(
                table: "Events",
                keyColumn: "Id",
                keyValue: 1,
                column: "Termin",
                value: new DateTime(2025, 12, 16, 0, 0, 0, 0, DateTimeKind.Local));

            migrationBuilder.UpdateData(
                table: "PortalVideo",
                keyColumn: "Id",
                keyValue: 1,
                column: "UploadDate",
                value: new DateTime(2025, 12, 9, 13, 18, 16, 996, DateTimeKind.Local).AddTicks(796));

            migrationBuilder.UpdateData(
                table: "PromptAiSettings",
                keyColumn: "Id",
                keyValue: 1,
                column: "UpdatedAt",
                value: new DateTime(2025, 12, 9, 12, 18, 16, 996, DateTimeKind.Utc).AddTicks(141));

            migrationBuilder.UpdateData(
                table: "PromptTypeGuidances",
                keyColumn: "Id",
                keyValue: 1,
                column: "UpdatedAt",
                value: new DateTime(2025, 12, 9, 12, 18, 16, 996, DateTimeKind.Utc).AddTicks(173));

            migrationBuilder.UpdateData(
                table: "PromptTypeGuidances",
                keyColumn: "Id",
                keyValue: 2,
                column: "UpdatedAt",
                value: new DateTime(2025, 12, 9, 12, 18, 16, 996, DateTimeKind.Utc).AddTicks(173));

            migrationBuilder.UpdateData(
                table: "PromptTypeGuidances",
                keyColumn: "Id",
                keyValue: 3,
                column: "UpdatedAt",
                value: new DateTime(2025, 12, 9, 12, 18, 16, 996, DateTimeKind.Utc).AddTicks(174));

            migrationBuilder.UpdateData(
                table: "PromptTypeGuidances",
                keyColumn: "Id",
                keyValue: 4,
                column: "UpdatedAt",
                value: new DateTime(2025, 12, 9, 12, 18, 16, 996, DateTimeKind.Utc).AddTicks(175));

            migrationBuilder.UpdateData(
                table: "PromptTypeGuidances",
                keyColumn: "Id",
                keyValue: 5,
                column: "UpdatedAt",
                value: new DateTime(2025, 12, 9, 12, 18, 16, 996, DateTimeKind.Utc).AddTicks(176));

            migrationBuilder.UpdateData(
                table: "PromptTypeGuidances",
                keyColumn: "Id",
                keyValue: 6,
                column: "UpdatedAt",
                value: new DateTime(2025, 12, 9, 12, 18, 16, 996, DateTimeKind.Utc).AddTicks(177));

            migrationBuilder.UpdateData(
                table: "PromptTypeGuidances",
                keyColumn: "Id",
                keyValue: 7,
                column: "UpdatedAt",
                value: new DateTime(2025, 12, 9, 12, 18, 16, 996, DateTimeKind.Utc).AddTicks(178));

            migrationBuilder.UpdateData(
                table: "Skills",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2025, 12, 9, 13, 18, 16, 996, DateTimeKind.Local).AddTicks(476));
        }
    }
}
