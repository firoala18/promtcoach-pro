using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace ProjectsWebApp.DataAccsess.Migrations
{
    /// <inheritdoc />
    public partial class AddPromptExternShareFunc : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PromptShareLinks",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    PromptTemplateId = table.Column<int>(type: "integer", nullable: false),
                    Group = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    PublicId = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    CreatedByUserId = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    ExpiresAtUtc = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PromptShareLinks", x => x.Id);
                });

            migrationBuilder.UpdateData(
                table: "Events",
                keyColumn: "Id",
                keyValue: 1,
                column: "Termin",
                value: new DateTime(2025, 12, 10, 0, 0, 0, 0, DateTimeKind.Local));

            migrationBuilder.UpdateData(
                table: "PortalVideo",
                keyColumn: "Id",
                keyValue: 1,
                column: "UploadDate",
                value: new DateTime(2025, 12, 3, 12, 36, 54, 347, DateTimeKind.Local).AddTicks(2100));

            migrationBuilder.UpdateData(
                table: "PromptAiSettings",
                keyColumn: "Id",
                keyValue: 1,
                column: "UpdatedAt",
                value: new DateTime(2025, 12, 3, 11, 36, 54, 347, DateTimeKind.Utc).AddTicks(1546));

            migrationBuilder.UpdateData(
                table: "PromptTypeGuidances",
                keyColumn: "Id",
                keyValue: 1,
                column: "UpdatedAt",
                value: new DateTime(2025, 12, 3, 11, 36, 54, 347, DateTimeKind.Utc).AddTicks(1574));

            migrationBuilder.UpdateData(
                table: "PromptTypeGuidances",
                keyColumn: "Id",
                keyValue: 2,
                column: "UpdatedAt",
                value: new DateTime(2025, 12, 3, 11, 36, 54, 347, DateTimeKind.Utc).AddTicks(1576));

            migrationBuilder.UpdateData(
                table: "PromptTypeGuidances",
                keyColumn: "Id",
                keyValue: 3,
                column: "UpdatedAt",
                value: new DateTime(2025, 12, 3, 11, 36, 54, 347, DateTimeKind.Utc).AddTicks(1576));

            migrationBuilder.UpdateData(
                table: "PromptTypeGuidances",
                keyColumn: "Id",
                keyValue: 4,
                column: "UpdatedAt",
                value: new DateTime(2025, 12, 3, 11, 36, 54, 347, DateTimeKind.Utc).AddTicks(1577));

            migrationBuilder.UpdateData(
                table: "PromptTypeGuidances",
                keyColumn: "Id",
                keyValue: 5,
                column: "UpdatedAt",
                value: new DateTime(2025, 12, 3, 11, 36, 54, 347, DateTimeKind.Utc).AddTicks(1578));

            migrationBuilder.UpdateData(
                table: "PromptTypeGuidances",
                keyColumn: "Id",
                keyValue: 6,
                column: "UpdatedAt",
                value: new DateTime(2025, 12, 3, 11, 36, 54, 347, DateTimeKind.Utc).AddTicks(1579));

            migrationBuilder.UpdateData(
                table: "PromptTypeGuidances",
                keyColumn: "Id",
                keyValue: 7,
                column: "UpdatedAt",
                value: new DateTime(2025, 12, 3, 11, 36, 54, 347, DateTimeKind.Utc).AddTicks(1580));

            migrationBuilder.UpdateData(
                table: "Skills",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2025, 12, 3, 12, 36, 54, 347, DateTimeKind.Local).AddTicks(1837));

            migrationBuilder.CreateIndex(
                name: "IX_PromptShareLinks_PublicId",
                table: "PromptShareLinks",
                column: "PublicId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PromptShareLinks");

            migrationBuilder.UpdateData(
                table: "Events",
                keyColumn: "Id",
                keyValue: 1,
                column: "Termin",
                value: new DateTime(2025, 12, 9, 0, 0, 0, 0, DateTimeKind.Local));

            migrationBuilder.UpdateData(
                table: "PortalVideo",
                keyColumn: "Id",
                keyValue: 1,
                column: "UploadDate",
                value: new DateTime(2025, 12, 2, 14, 29, 1, 41, DateTimeKind.Local).AddTicks(2597));

            migrationBuilder.UpdateData(
                table: "PromptAiSettings",
                keyColumn: "Id",
                keyValue: 1,
                column: "UpdatedAt",
                value: new DateTime(2025, 12, 2, 13, 29, 1, 41, DateTimeKind.Utc).AddTicks(1996));

            migrationBuilder.UpdateData(
                table: "PromptTypeGuidances",
                keyColumn: "Id",
                keyValue: 1,
                column: "UpdatedAt",
                value: new DateTime(2025, 12, 2, 13, 29, 1, 41, DateTimeKind.Utc).AddTicks(2026));

            migrationBuilder.UpdateData(
                table: "PromptTypeGuidances",
                keyColumn: "Id",
                keyValue: 2,
                column: "UpdatedAt",
                value: new DateTime(2025, 12, 2, 13, 29, 1, 41, DateTimeKind.Utc).AddTicks(2027));

            migrationBuilder.UpdateData(
                table: "PromptTypeGuidances",
                keyColumn: "Id",
                keyValue: 3,
                column: "UpdatedAt",
                value: new DateTime(2025, 12, 2, 13, 29, 1, 41, DateTimeKind.Utc).AddTicks(2028));

            migrationBuilder.UpdateData(
                table: "PromptTypeGuidances",
                keyColumn: "Id",
                keyValue: 4,
                column: "UpdatedAt",
                value: new DateTime(2025, 12, 2, 13, 29, 1, 41, DateTimeKind.Utc).AddTicks(2029));

            migrationBuilder.UpdateData(
                table: "PromptTypeGuidances",
                keyColumn: "Id",
                keyValue: 5,
                column: "UpdatedAt",
                value: new DateTime(2025, 12, 2, 13, 29, 1, 41, DateTimeKind.Utc).AddTicks(2030));

            migrationBuilder.UpdateData(
                table: "PromptTypeGuidances",
                keyColumn: "Id",
                keyValue: 6,
                column: "UpdatedAt",
                value: new DateTime(2025, 12, 2, 13, 29, 1, 41, DateTimeKind.Utc).AddTicks(2030));

            migrationBuilder.UpdateData(
                table: "PromptTypeGuidances",
                keyColumn: "Id",
                keyValue: 7,
                column: "UpdatedAt",
                value: new DateTime(2025, 12, 2, 13, 29, 1, 41, DateTimeKind.Utc).AddTicks(2031));

            migrationBuilder.UpdateData(
                table: "Skills",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2025, 12, 2, 14, 29, 1, 41, DateTimeKind.Local).AddTicks(2304));
        }
    }
}
