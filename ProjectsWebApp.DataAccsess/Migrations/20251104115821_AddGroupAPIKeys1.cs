using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProjectsWebApp.DataAccsess.Migrations
{
    /// <inheritdoc />
    public partial class AddGroupAPIKeys1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ClaudeModel",
                table: "GroupApiKeySettings",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "GeminiModel",
                table: "GroupApiKeySettings",
                type: "text",
                nullable: true);

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ClaudeModel",
                table: "GroupApiKeySettings");

            migrationBuilder.DropColumn(
                name: "GeminiModel",
                table: "GroupApiKeySettings");

            migrationBuilder.UpdateData(
                table: "PortalVideo",
                keyColumn: "Id",
                keyValue: 1,
                column: "UploadDate",
                value: new DateTime(2025, 11, 4, 11, 56, 4, 501, DateTimeKind.Local).AddTicks(163));

            migrationBuilder.UpdateData(
                table: "PromptAiSettings",
                keyColumn: "Id",
                keyValue: 1,
                column: "UpdatedAt",
                value: new DateTime(2025, 11, 4, 10, 56, 4, 500, DateTimeKind.Utc).AddTicks(9556));

            migrationBuilder.UpdateData(
                table: "PromptTypeGuidances",
                keyColumn: "Id",
                keyValue: 1,
                column: "UpdatedAt",
                value: new DateTime(2025, 11, 4, 10, 56, 4, 500, DateTimeKind.Utc).AddTicks(9584));

            migrationBuilder.UpdateData(
                table: "PromptTypeGuidances",
                keyColumn: "Id",
                keyValue: 2,
                column: "UpdatedAt",
                value: new DateTime(2025, 11, 4, 10, 56, 4, 500, DateTimeKind.Utc).AddTicks(9585));

            migrationBuilder.UpdateData(
                table: "PromptTypeGuidances",
                keyColumn: "Id",
                keyValue: 3,
                column: "UpdatedAt",
                value: new DateTime(2025, 11, 4, 10, 56, 4, 500, DateTimeKind.Utc).AddTicks(9585));

            migrationBuilder.UpdateData(
                table: "PromptTypeGuidances",
                keyColumn: "Id",
                keyValue: 4,
                column: "UpdatedAt",
                value: new DateTime(2025, 11, 4, 10, 56, 4, 500, DateTimeKind.Utc).AddTicks(9586));

            migrationBuilder.UpdateData(
                table: "PromptTypeGuidances",
                keyColumn: "Id",
                keyValue: 5,
                column: "UpdatedAt",
                value: new DateTime(2025, 11, 4, 10, 56, 4, 500, DateTimeKind.Utc).AddTicks(9587));

            migrationBuilder.UpdateData(
                table: "PromptTypeGuidances",
                keyColumn: "Id",
                keyValue: 6,
                column: "UpdatedAt",
                value: new DateTime(2025, 11, 4, 10, 56, 4, 500, DateTimeKind.Utc).AddTicks(9588));

            migrationBuilder.UpdateData(
                table: "PromptTypeGuidances",
                keyColumn: "Id",
                keyValue: 7,
                column: "UpdatedAt",
                value: new DateTime(2025, 11, 4, 10, 56, 4, 500, DateTimeKind.Utc).AddTicks(9588));

            migrationBuilder.UpdateData(
                table: "Skills",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 4, 11, 56, 4, 500, DateTimeKind.Local).AddTicks(9897));
        }
    }
}
