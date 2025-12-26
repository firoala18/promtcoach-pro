using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProjectsWebApp.DataAccsess.Migrations
{
    /// <inheritdoc />
    public partial class AddAssistantReflektion : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Reflektion",
                table: "Assistants",
                type: "character varying(4000)",
                maxLength: 4000,
                nullable: true);

            migrationBuilder.UpdateData(
                table: "Events",
                keyColumn: "Id",
                keyValue: 1,
                column: "Termin",
                value: new DateTime(2025, 12, 2, 0, 0, 0, 0, DateTimeKind.Local));

            migrationBuilder.UpdateData(
                table: "PortalVideo",
                keyColumn: "Id",
                keyValue: 1,
                column: "UploadDate",
                value: new DateTime(2025, 11, 25, 8, 59, 12, 73, DateTimeKind.Local).AddTicks(3416));

            migrationBuilder.UpdateData(
                table: "PromptAiSettings",
                keyColumn: "Id",
                keyValue: 1,
                column: "UpdatedAt",
                value: new DateTime(2025, 11, 25, 7, 59, 12, 73, DateTimeKind.Utc).AddTicks(2763));

            migrationBuilder.UpdateData(
                table: "PromptTypeGuidances",
                keyColumn: "Id",
                keyValue: 1,
                column: "UpdatedAt",
                value: new DateTime(2025, 11, 25, 7, 59, 12, 73, DateTimeKind.Utc).AddTicks(2788));

            migrationBuilder.UpdateData(
                table: "PromptTypeGuidances",
                keyColumn: "Id",
                keyValue: 2,
                column: "UpdatedAt",
                value: new DateTime(2025, 11, 25, 7, 59, 12, 73, DateTimeKind.Utc).AddTicks(2790));

            migrationBuilder.UpdateData(
                table: "PromptTypeGuidances",
                keyColumn: "Id",
                keyValue: 3,
                column: "UpdatedAt",
                value: new DateTime(2025, 11, 25, 7, 59, 12, 73, DateTimeKind.Utc).AddTicks(2791));

            migrationBuilder.UpdateData(
                table: "PromptTypeGuidances",
                keyColumn: "Id",
                keyValue: 4,
                column: "UpdatedAt",
                value: new DateTime(2025, 11, 25, 7, 59, 12, 73, DateTimeKind.Utc).AddTicks(2792));

            migrationBuilder.UpdateData(
                table: "PromptTypeGuidances",
                keyColumn: "Id",
                keyValue: 5,
                column: "UpdatedAt",
                value: new DateTime(2025, 11, 25, 7, 59, 12, 73, DateTimeKind.Utc).AddTicks(2793));

            migrationBuilder.UpdateData(
                table: "PromptTypeGuidances",
                keyColumn: "Id",
                keyValue: 6,
                column: "UpdatedAt",
                value: new DateTime(2025, 11, 25, 7, 59, 12, 73, DateTimeKind.Utc).AddTicks(2793));

            migrationBuilder.UpdateData(
                table: "PromptTypeGuidances",
                keyColumn: "Id",
                keyValue: 7,
                column: "UpdatedAt",
                value: new DateTime(2025, 11, 25, 7, 59, 12, 73, DateTimeKind.Utc).AddTicks(2794));

            migrationBuilder.UpdateData(
                table: "Skills",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 25, 8, 59, 12, 73, DateTimeKind.Local).AddTicks(3128));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Reflektion",
                table: "Assistants");

            migrationBuilder.UpdateData(
                table: "Events",
                keyColumn: "Id",
                keyValue: 1,
                column: "Termin",
                value: new DateTime(2025, 11, 27, 0, 0, 0, 0, DateTimeKind.Local));

            migrationBuilder.UpdateData(
                table: "PortalVideo",
                keyColumn: "Id",
                keyValue: 1,
                column: "UploadDate",
                value: new DateTime(2025, 11, 20, 15, 40, 49, 740, DateTimeKind.Local).AddTicks(6093));

            migrationBuilder.UpdateData(
                table: "PromptAiSettings",
                keyColumn: "Id",
                keyValue: 1,
                column: "UpdatedAt",
                value: new DateTime(2025, 11, 20, 14, 40, 49, 740, DateTimeKind.Utc).AddTicks(5532));

            migrationBuilder.UpdateData(
                table: "PromptTypeGuidances",
                keyColumn: "Id",
                keyValue: 1,
                column: "UpdatedAt",
                value: new DateTime(2025, 11, 20, 14, 40, 49, 740, DateTimeKind.Utc).AddTicks(5557));

            migrationBuilder.UpdateData(
                table: "PromptTypeGuidances",
                keyColumn: "Id",
                keyValue: 2,
                column: "UpdatedAt",
                value: new DateTime(2025, 11, 20, 14, 40, 49, 740, DateTimeKind.Utc).AddTicks(5558));

            migrationBuilder.UpdateData(
                table: "PromptTypeGuidances",
                keyColumn: "Id",
                keyValue: 3,
                column: "UpdatedAt",
                value: new DateTime(2025, 11, 20, 14, 40, 49, 740, DateTimeKind.Utc).AddTicks(5559));

            migrationBuilder.UpdateData(
                table: "PromptTypeGuidances",
                keyColumn: "Id",
                keyValue: 4,
                column: "UpdatedAt",
                value: new DateTime(2025, 11, 20, 14, 40, 49, 740, DateTimeKind.Utc).AddTicks(5560));

            migrationBuilder.UpdateData(
                table: "PromptTypeGuidances",
                keyColumn: "Id",
                keyValue: 5,
                column: "UpdatedAt",
                value: new DateTime(2025, 11, 20, 14, 40, 49, 740, DateTimeKind.Utc).AddTicks(5561));

            migrationBuilder.UpdateData(
                table: "PromptTypeGuidances",
                keyColumn: "Id",
                keyValue: 6,
                column: "UpdatedAt",
                value: new DateTime(2025, 11, 20, 14, 40, 49, 740, DateTimeKind.Utc).AddTicks(5561));

            migrationBuilder.UpdateData(
                table: "PromptTypeGuidances",
                keyColumn: "Id",
                keyValue: 7,
                column: "UpdatedAt",
                value: new DateTime(2025, 11, 20, 14, 40, 49, 740, DateTimeKind.Utc).AddTicks(5562));

            migrationBuilder.UpdateData(
                table: "Skills",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 20, 15, 40, 49, 740, DateTimeKind.Local).AddTicks(5828));
        }
    }
}
