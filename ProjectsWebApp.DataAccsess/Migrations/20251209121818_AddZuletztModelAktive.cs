using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProjectsWebApp.DataAccsess.Migrations
{
    /// <inheritdoc />
    public partial class AddZuletztModelAktive : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "GeminiUpdatedAt",
                table: "ApiKeySettings",
                type: "timestamp without time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "KisskiUpdatedAt",
                table: "ApiKeySettings",
                type: "timestamp without time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "OpenAIUpdatedAt",
                table: "ApiKeySettings",
                type: "timestamp without time zone",
                nullable: true);

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "GeminiUpdatedAt",
                table: "ApiKeySettings");

            migrationBuilder.DropColumn(
                name: "KisskiUpdatedAt",
                table: "ApiKeySettings");

            migrationBuilder.DropColumn(
                name: "OpenAIUpdatedAt",
                table: "ApiKeySettings");

            migrationBuilder.UpdateData(
                table: "PortalVideo",
                keyColumn: "Id",
                keyValue: 1,
                column: "UploadDate",
                value: new DateTime(2025, 12, 9, 10, 25, 58, 985, DateTimeKind.Local).AddTicks(1318));

            migrationBuilder.UpdateData(
                table: "PromptAiSettings",
                keyColumn: "Id",
                keyValue: 1,
                column: "UpdatedAt",
                value: new DateTime(2025, 12, 9, 9, 25, 58, 985, DateTimeKind.Utc).AddTicks(744));

            migrationBuilder.UpdateData(
                table: "PromptTypeGuidances",
                keyColumn: "Id",
                keyValue: 1,
                column: "UpdatedAt",
                value: new DateTime(2025, 12, 9, 9, 25, 58, 985, DateTimeKind.Utc).AddTicks(769));

            migrationBuilder.UpdateData(
                table: "PromptTypeGuidances",
                keyColumn: "Id",
                keyValue: 2,
                column: "UpdatedAt",
                value: new DateTime(2025, 12, 9, 9, 25, 58, 985, DateTimeKind.Utc).AddTicks(770));

            migrationBuilder.UpdateData(
                table: "PromptTypeGuidances",
                keyColumn: "Id",
                keyValue: 3,
                column: "UpdatedAt",
                value: new DateTime(2025, 12, 9, 9, 25, 58, 985, DateTimeKind.Utc).AddTicks(770));

            migrationBuilder.UpdateData(
                table: "PromptTypeGuidances",
                keyColumn: "Id",
                keyValue: 4,
                column: "UpdatedAt",
                value: new DateTime(2025, 12, 9, 9, 25, 58, 985, DateTimeKind.Utc).AddTicks(771));

            migrationBuilder.UpdateData(
                table: "PromptTypeGuidances",
                keyColumn: "Id",
                keyValue: 5,
                column: "UpdatedAt",
                value: new DateTime(2025, 12, 9, 9, 25, 58, 985, DateTimeKind.Utc).AddTicks(772));

            migrationBuilder.UpdateData(
                table: "PromptTypeGuidances",
                keyColumn: "Id",
                keyValue: 6,
                column: "UpdatedAt",
                value: new DateTime(2025, 12, 9, 9, 25, 58, 985, DateTimeKind.Utc).AddTicks(773));

            migrationBuilder.UpdateData(
                table: "PromptTypeGuidances",
                keyColumn: "Id",
                keyValue: 7,
                column: "UpdatedAt",
                value: new DateTime(2025, 12, 9, 9, 25, 58, 985, DateTimeKind.Utc).AddTicks(774));

            migrationBuilder.UpdateData(
                table: "Skills",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2025, 12, 9, 10, 25, 58, 985, DateTimeKind.Local).AddTicks(1059));
        }
    }
}
