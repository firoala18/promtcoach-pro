using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace ProjectsWebApp.DataAccsess.Migrations
{
    /// <inheritdoc />
    public partial class AddGroupTypeGuidance : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "GroupPromptTypeGuidances",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Group = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Type = table.Column<int>(type: "integer", nullable: false),
                    GuidanceText = table.Column<string>(type: "text", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GroupPromptTypeGuidances", x => x.Id);
                });

            migrationBuilder.UpdateData(
                table: "ContactMessageSettings",
                keyColumn: "Id",
                keyValue: 1,
                column: "Message",
                value: "Um unser Prompt-Engineering-Tool auszuprobieren,\r\nkontaktieren Sie uns bitte per E-Mail.");

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
                value: new DateTime(2025, 11, 20, 13, 1, 17, 886, DateTimeKind.Local).AddTicks(2698));

            migrationBuilder.UpdateData(
                table: "PromptAiSettings",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "SystemPreamble", "UpdatedAt" },
                values: new object[] { "Du bist eine expert*innengeführte Assistenz für Instructional Design, Hochschuldidaktik und wissenschaftliches Arbeiten.\r\nSchreibe in präzisem, formalem Deutsch für Forschung & Lehre. Nutze wissenschaftliche Terminologie.\r\nHalte dich strikt an das JSON-Schema (Structured Outputs). Antworte ausschließlich mit JSON.\r\nJedes Filter-Item muss eine überprüfbare Leistung erzeugen (Artefakt/Metrik/Kriterium, Quellenstandard).", new DateTime(2025, 11, 20, 12, 1, 17, 886, DateTimeKind.Utc).AddTicks(2091) });

            migrationBuilder.UpdateData(
                table: "PromptTypeGuidances",
                keyColumn: "Id",
                keyValue: 1,
                column: "UpdatedAt",
                value: new DateTime(2025, 11, 20, 12, 1, 17, 886, DateTimeKind.Utc).AddTicks(2118));

            migrationBuilder.UpdateData(
                table: "PromptTypeGuidances",
                keyColumn: "Id",
                keyValue: 2,
                column: "UpdatedAt",
                value: new DateTime(2025, 11, 20, 12, 1, 17, 886, DateTimeKind.Utc).AddTicks(2119));

            migrationBuilder.UpdateData(
                table: "PromptTypeGuidances",
                keyColumn: "Id",
                keyValue: 3,
                column: "UpdatedAt",
                value: new DateTime(2025, 11, 20, 12, 1, 17, 886, DateTimeKind.Utc).AddTicks(2120));

            migrationBuilder.UpdateData(
                table: "PromptTypeGuidances",
                keyColumn: "Id",
                keyValue: 4,
                column: "UpdatedAt",
                value: new DateTime(2025, 11, 20, 12, 1, 17, 886, DateTimeKind.Utc).AddTicks(2121));

            migrationBuilder.UpdateData(
                table: "PromptTypeGuidances",
                keyColumn: "Id",
                keyValue: 5,
                column: "UpdatedAt",
                value: new DateTime(2025, 11, 20, 12, 1, 17, 886, DateTimeKind.Utc).AddTicks(2122));

            migrationBuilder.UpdateData(
                table: "PromptTypeGuidances",
                keyColumn: "Id",
                keyValue: 6,
                column: "UpdatedAt",
                value: new DateTime(2025, 11, 20, 12, 1, 17, 886, DateTimeKind.Utc).AddTicks(2123));

            migrationBuilder.UpdateData(
                table: "PromptTypeGuidances",
                keyColumn: "Id",
                keyValue: 7,
                column: "UpdatedAt",
                value: new DateTime(2025, 11, 20, 12, 1, 17, 886, DateTimeKind.Utc).AddTicks(2124));

            migrationBuilder.UpdateData(
                table: "Skills",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 20, 13, 1, 17, 886, DateTimeKind.Local).AddTicks(2394));

            migrationBuilder.CreateIndex(
                name: "IX_GroupPromptTypeGuidances_Group_Type",
                table: "GroupPromptTypeGuidances",
                columns: new[] { "Group", "Type" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "GroupPromptTypeGuidances");

            migrationBuilder.UpdateData(
                table: "ContactMessageSettings",
                keyColumn: "Id",
                keyValue: 1,
                column: "Message",
                value: "Um unser Prompt-Engineering-Tool auszuprobieren,\nkontaktieren Sie uns bitte per E-Mail.");

            migrationBuilder.UpdateData(
                table: "Events",
                keyColumn: "Id",
                keyValue: 1,
                column: "Termin",
                value: new DateTime(2025, 11, 21, 0, 0, 0, 0, DateTimeKind.Local));

            migrationBuilder.UpdateData(
                table: "PortalVideo",
                keyColumn: "Id",
                keyValue: 1,
                column: "UploadDate",
                value: new DateTime(2025, 11, 14, 13, 9, 4, 615, DateTimeKind.Local).AddTicks(6639));

            migrationBuilder.UpdateData(
                table: "PromptAiSettings",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "SystemPreamble", "UpdatedAt" },
                values: new object[] { "Du bist eine expert*innengeführte Assistenz für Instructional Design, Hochschuldidaktik und wissenschaftliches Arbeiten.\nSchreibe in präzisem, formalem Deutsch für Forschung & Lehre. Nutze wissenschaftliche Terminologie.\nHalte dich strikt an das JSON-Schema (Structured Outputs). Antworte ausschließlich mit JSON.\nJedes Filter-Item muss eine überprüfbare Leistung erzeugen (Artefakt/Metrik/Kriterium, Quellenstandard).", new DateTime(2025, 11, 14, 12, 9, 4, 615, DateTimeKind.Utc).AddTicks(6046) });

            migrationBuilder.UpdateData(
                table: "PromptTypeGuidances",
                keyColumn: "Id",
                keyValue: 1,
                column: "UpdatedAt",
                value: new DateTime(2025, 11, 14, 12, 9, 4, 615, DateTimeKind.Utc).AddTicks(6067));

            migrationBuilder.UpdateData(
                table: "PromptTypeGuidances",
                keyColumn: "Id",
                keyValue: 2,
                column: "UpdatedAt",
                value: new DateTime(2025, 11, 14, 12, 9, 4, 615, DateTimeKind.Utc).AddTicks(6120));

            migrationBuilder.UpdateData(
                table: "PromptTypeGuidances",
                keyColumn: "Id",
                keyValue: 3,
                column: "UpdatedAt",
                value: new DateTime(2025, 11, 14, 12, 9, 4, 615, DateTimeKind.Utc).AddTicks(6121));

            migrationBuilder.UpdateData(
                table: "PromptTypeGuidances",
                keyColumn: "Id",
                keyValue: 4,
                column: "UpdatedAt",
                value: new DateTime(2025, 11, 14, 12, 9, 4, 615, DateTimeKind.Utc).AddTicks(6122));

            migrationBuilder.UpdateData(
                table: "PromptTypeGuidances",
                keyColumn: "Id",
                keyValue: 5,
                column: "UpdatedAt",
                value: new DateTime(2025, 11, 14, 12, 9, 4, 615, DateTimeKind.Utc).AddTicks(6122));

            migrationBuilder.UpdateData(
                table: "PromptTypeGuidances",
                keyColumn: "Id",
                keyValue: 6,
                column: "UpdatedAt",
                value: new DateTime(2025, 11, 14, 12, 9, 4, 615, DateTimeKind.Utc).AddTicks(6123));

            migrationBuilder.UpdateData(
                table: "PromptTypeGuidances",
                keyColumn: "Id",
                keyValue: 7,
                column: "UpdatedAt",
                value: new DateTime(2025, 11, 14, 12, 9, 4, 615, DateTimeKind.Utc).AddTicks(6124));

            migrationBuilder.UpdateData(
                table: "Skills",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 14, 13, 9, 4, 615, DateTimeKind.Local).AddTicks(6352));
        }
    }
}
