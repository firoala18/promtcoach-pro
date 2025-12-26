using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProjectsWebApp.DataAccsess.Migrations
{
    /// <inheritdoc />
    public partial class AddGeneralAPIChoices1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ActiveProvider",
                table: "ApiKeySettings",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "GeminiApiKey",
                table: "ApiKeySettings",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "GeminiModel",
                table: "ApiKeySettings",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "KisskiApiKey",
                table: "ApiKeySettings",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "KisskiBaseUrl",
                table: "ApiKeySettings",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "KisskiModel",
                table: "ApiKeySettings",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OpenAIBaseUrl",
                table: "ApiKeySettings",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OpenAIModel",
                table: "ApiKeySettings",
                type: "text",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "PortalVideo",
                keyColumn: "Id",
                keyValue: 1,
                column: "UploadDate",
                value: new DateTime(2025, 11, 7, 10, 50, 25, 396, DateTimeKind.Local).AddTicks(1649));

            migrationBuilder.UpdateData(
                table: "PromptAiSettings",
                keyColumn: "Id",
                keyValue: 1,
                column: "UpdatedAt",
                value: new DateTime(2025, 11, 7, 9, 50, 25, 396, DateTimeKind.Utc).AddTicks(1138));

            migrationBuilder.UpdateData(
                table: "PromptTypeGuidances",
                keyColumn: "Id",
                keyValue: 1,
                column: "UpdatedAt",
                value: new DateTime(2025, 11, 7, 9, 50, 25, 396, DateTimeKind.Utc).AddTicks(1165));

            migrationBuilder.UpdateData(
                table: "PromptTypeGuidances",
                keyColumn: "Id",
                keyValue: 2,
                column: "UpdatedAt",
                value: new DateTime(2025, 11, 7, 9, 50, 25, 396, DateTimeKind.Utc).AddTicks(1166));

            migrationBuilder.UpdateData(
                table: "PromptTypeGuidances",
                keyColumn: "Id",
                keyValue: 3,
                column: "UpdatedAt",
                value: new DateTime(2025, 11, 7, 9, 50, 25, 396, DateTimeKind.Utc).AddTicks(1166));

            migrationBuilder.UpdateData(
                table: "PromptTypeGuidances",
                keyColumn: "Id",
                keyValue: 4,
                column: "UpdatedAt",
                value: new DateTime(2025, 11, 7, 9, 50, 25, 396, DateTimeKind.Utc).AddTicks(1167));

            migrationBuilder.UpdateData(
                table: "PromptTypeGuidances",
                keyColumn: "Id",
                keyValue: 5,
                column: "UpdatedAt",
                value: new DateTime(2025, 11, 7, 9, 50, 25, 396, DateTimeKind.Utc).AddTicks(1168));

            migrationBuilder.UpdateData(
                table: "PromptTypeGuidances",
                keyColumn: "Id",
                keyValue: 6,
                column: "UpdatedAt",
                value: new DateTime(2025, 11, 7, 9, 50, 25, 396, DateTimeKind.Utc).AddTicks(1168));

            migrationBuilder.UpdateData(
                table: "PromptTypeGuidances",
                keyColumn: "Id",
                keyValue: 7,
                column: "UpdatedAt",
                value: new DateTime(2025, 11, 7, 9, 50, 25, 396, DateTimeKind.Utc).AddTicks(1170));

            migrationBuilder.UpdateData(
                table: "Skills",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 7, 10, 50, 25, 396, DateTimeKind.Local).AddTicks(1389));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ActiveProvider",
                table: "ApiKeySettings");

            migrationBuilder.DropColumn(
                name: "GeminiApiKey",
                table: "ApiKeySettings");

            migrationBuilder.DropColumn(
                name: "GeminiModel",
                table: "ApiKeySettings");

            migrationBuilder.DropColumn(
                name: "KisskiApiKey",
                table: "ApiKeySettings");

            migrationBuilder.DropColumn(
                name: "KisskiBaseUrl",
                table: "ApiKeySettings");

            migrationBuilder.DropColumn(
                name: "KisskiModel",
                table: "ApiKeySettings");

            migrationBuilder.DropColumn(
                name: "OpenAIBaseUrl",
                table: "ApiKeySettings");

            migrationBuilder.DropColumn(
                name: "OpenAIModel",
                table: "ApiKeySettings");

            migrationBuilder.UpdateData(
                table: "PortalVideo",
                keyColumn: "Id",
                keyValue: 1,
                column: "UploadDate",
                value: new DateTime(2025, 11, 7, 9, 13, 2, 662, DateTimeKind.Local).AddTicks(9101));

            migrationBuilder.UpdateData(
                table: "PromptAiSettings",
                keyColumn: "Id",
                keyValue: 1,
                column: "UpdatedAt",
                value: new DateTime(2025, 11, 7, 8, 13, 2, 662, DateTimeKind.Utc).AddTicks(8602));

            migrationBuilder.UpdateData(
                table: "PromptTypeGuidances",
                keyColumn: "Id",
                keyValue: 1,
                column: "UpdatedAt",
                value: new DateTime(2025, 11, 7, 8, 13, 2, 662, DateTimeKind.Utc).AddTicks(8628));

            migrationBuilder.UpdateData(
                table: "PromptTypeGuidances",
                keyColumn: "Id",
                keyValue: 2,
                column: "UpdatedAt",
                value: new DateTime(2025, 11, 7, 8, 13, 2, 662, DateTimeKind.Utc).AddTicks(8629));

            migrationBuilder.UpdateData(
                table: "PromptTypeGuidances",
                keyColumn: "Id",
                keyValue: 3,
                column: "UpdatedAt",
                value: new DateTime(2025, 11, 7, 8, 13, 2, 662, DateTimeKind.Utc).AddTicks(8629));

            migrationBuilder.UpdateData(
                table: "PromptTypeGuidances",
                keyColumn: "Id",
                keyValue: 4,
                column: "UpdatedAt",
                value: new DateTime(2025, 11, 7, 8, 13, 2, 662, DateTimeKind.Utc).AddTicks(8630));

            migrationBuilder.UpdateData(
                table: "PromptTypeGuidances",
                keyColumn: "Id",
                keyValue: 5,
                column: "UpdatedAt",
                value: new DateTime(2025, 11, 7, 8, 13, 2, 662, DateTimeKind.Utc).AddTicks(8631));

            migrationBuilder.UpdateData(
                table: "PromptTypeGuidances",
                keyColumn: "Id",
                keyValue: 6,
                column: "UpdatedAt",
                value: new DateTime(2025, 11, 7, 8, 13, 2, 662, DateTimeKind.Utc).AddTicks(8632));

            migrationBuilder.UpdateData(
                table: "PromptTypeGuidances",
                keyColumn: "Id",
                keyValue: 7,
                column: "UpdatedAt",
                value: new DateTime(2025, 11, 7, 8, 13, 2, 662, DateTimeKind.Utc).AddTicks(8632));

            migrationBuilder.UpdateData(
                table: "Skills",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 7, 9, 13, 2, 662, DateTimeKind.Local).AddTicks(8872));
        }
    }
}
