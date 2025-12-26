using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProjectsWebApp.DataAccsess.Migrations
{
    /// <inheritdoc />
    public partial class AddGroupAPIKeys2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "KisskiModel",
                table: "GroupApiKeySettings",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OpenAIModel",
                table: "GroupApiKeySettings",
                type: "text",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "PortalVideo",
                keyColumn: "Id",
                keyValue: 1,
                column: "UploadDate",
                value: new DateTime(2025, 11, 4, 13, 45, 26, 83, DateTimeKind.Local).AddTicks(544));

            migrationBuilder.UpdateData(
                table: "PromptAiSettings",
                keyColumn: "Id",
                keyValue: 1,
                column: "UpdatedAt",
                value: new DateTime(2025, 11, 4, 12, 45, 26, 83, DateTimeKind.Utc).AddTicks(20));

            migrationBuilder.UpdateData(
                table: "PromptTypeGuidances",
                keyColumn: "Id",
                keyValue: 1,
                column: "UpdatedAt",
                value: new DateTime(2025, 11, 4, 12, 45, 26, 83, DateTimeKind.Utc).AddTicks(48));

            migrationBuilder.UpdateData(
                table: "PromptTypeGuidances",
                keyColumn: "Id",
                keyValue: 2,
                column: "UpdatedAt",
                value: new DateTime(2025, 11, 4, 12, 45, 26, 83, DateTimeKind.Utc).AddTicks(49));

            migrationBuilder.UpdateData(
                table: "PromptTypeGuidances",
                keyColumn: "Id",
                keyValue: 3,
                column: "UpdatedAt",
                value: new DateTime(2025, 11, 4, 12, 45, 26, 83, DateTimeKind.Utc).AddTicks(50));

            migrationBuilder.UpdateData(
                table: "PromptTypeGuidances",
                keyColumn: "Id",
                keyValue: 4,
                column: "UpdatedAt",
                value: new DateTime(2025, 11, 4, 12, 45, 26, 83, DateTimeKind.Utc).AddTicks(51));

            migrationBuilder.UpdateData(
                table: "PromptTypeGuidances",
                keyColumn: "Id",
                keyValue: 5,
                column: "UpdatedAt",
                value: new DateTime(2025, 11, 4, 12, 45, 26, 83, DateTimeKind.Utc).AddTicks(51));

            migrationBuilder.UpdateData(
                table: "PromptTypeGuidances",
                keyColumn: "Id",
                keyValue: 6,
                column: "UpdatedAt",
                value: new DateTime(2025, 11, 4, 12, 45, 26, 83, DateTimeKind.Utc).AddTicks(52));

            migrationBuilder.UpdateData(
                table: "PromptTypeGuidances",
                keyColumn: "Id",
                keyValue: 7,
                column: "UpdatedAt",
                value: new DateTime(2025, 11, 4, 12, 45, 26, 83, DateTimeKind.Utc).AddTicks(53));

            migrationBuilder.UpdateData(
                table: "Skills",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 4, 13, 45, 26, 83, DateTimeKind.Local).AddTicks(299));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "KisskiModel",
                table: "GroupApiKeySettings");

            migrationBuilder.DropColumn(
                name: "OpenAIModel",
                table: "GroupApiKeySettings");

            migrationBuilder.UpdateData(
                table: "PortalVideo",
                keyColumn: "Id",
                keyValue: 1,
                column: "UploadDate",
                value: new DateTime(2025, 11, 4, 12, 58, 20, 854, DateTimeKind.Local).AddTicks(9647));

            migrationBuilder.UpdateData(
                table: "PromptAiSettings",
                keyColumn: "Id",
                keyValue: 1,
                column: "UpdatedAt",
                value: new DateTime(2025, 11, 4, 11, 58, 20, 854, DateTimeKind.Utc).AddTicks(9021));

            migrationBuilder.UpdateData(
                table: "PromptTypeGuidances",
                keyColumn: "Id",
                keyValue: 1,
                column: "UpdatedAt",
                value: new DateTime(2025, 11, 4, 11, 58, 20, 854, DateTimeKind.Utc).AddTicks(9049));

            migrationBuilder.UpdateData(
                table: "PromptTypeGuidances",
                keyColumn: "Id",
                keyValue: 2,
                column: "UpdatedAt",
                value: new DateTime(2025, 11, 4, 11, 58, 20, 854, DateTimeKind.Utc).AddTicks(9050));

            migrationBuilder.UpdateData(
                table: "PromptTypeGuidances",
                keyColumn: "Id",
                keyValue: 3,
                column: "UpdatedAt",
                value: new DateTime(2025, 11, 4, 11, 58, 20, 854, DateTimeKind.Utc).AddTicks(9051));

            migrationBuilder.UpdateData(
                table: "PromptTypeGuidances",
                keyColumn: "Id",
                keyValue: 4,
                column: "UpdatedAt",
                value: new DateTime(2025, 11, 4, 11, 58, 20, 854, DateTimeKind.Utc).AddTicks(9051));

            migrationBuilder.UpdateData(
                table: "PromptTypeGuidances",
                keyColumn: "Id",
                keyValue: 5,
                column: "UpdatedAt",
                value: new DateTime(2025, 11, 4, 11, 58, 20, 854, DateTimeKind.Utc).AddTicks(9052));

            migrationBuilder.UpdateData(
                table: "PromptTypeGuidances",
                keyColumn: "Id",
                keyValue: 6,
                column: "UpdatedAt",
                value: new DateTime(2025, 11, 4, 11, 58, 20, 854, DateTimeKind.Utc).AddTicks(9053));

            migrationBuilder.UpdateData(
                table: "PromptTypeGuidances",
                keyColumn: "Id",
                keyValue: 7,
                column: "UpdatedAt",
                value: new DateTime(2025, 11, 4, 11, 58, 20, 854, DateTimeKind.Utc).AddTicks(9053));

            migrationBuilder.UpdateData(
                table: "Skills",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 4, 12, 58, 20, 854, DateTimeKind.Local).AddTicks(9315));
        }
    }
}
