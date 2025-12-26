using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace ProjectsWebApp.DataAccsess.Migrations
{
    /// <inheritdoc />
    public partial class AddMultiGlobalSysPrompts : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "GlobalPromptConfigs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    SystemPreamble = table.Column<string>(type: "text", nullable: false),
                    UserInstruction = table.Column<string>(type: "text", nullable: false),
                    KiAssistantSystemPrompt = table.Column<string>(type: "text", nullable: false),
                    FilterSystemPreamble = table.Column<string>(type: "text", nullable: false),
                    FilterFirstLine = table.Column<string>(type: "text", nullable: false),
                    SmartSelectionSystemPreamble = table.Column<string>(type: "text", nullable: false),
                    SmartSelectionUserPrompt = table.Column<string>(type: "text", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GlobalPromptConfigs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "GlobalPromptConfigTypeGuidances",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    GlobalPromptConfigId = table.Column<int>(type: "integer", nullable: false),
                    Type = table.Column<int>(type: "integer", nullable: false),
                    GuidanceText = table.Column<string>(type: "text", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GlobalPromptConfigTypeGuidances", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GlobalPromptConfigTypeGuidances_GlobalPromptConfigs_GlobalP~",
                        column: x => x.GlobalPromptConfigId,
                        principalTable: "GlobalPromptConfigs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.UpdateData(
                table: "ContactMessageSettings",
                keyColumn: "Id",
                keyValue: 1,
                column: "Message",
                value: "Um unser Prompt-Engineering-Tool auszuprobieren,\nkontaktieren Sie uns bitte per E-Mail.");

            migrationBuilder.UpdateData(
                table: "PortalVideo",
                keyColumn: "Id",
                keyValue: 1,
                column: "UploadDate",
                value: new DateTime(2025, 11, 10, 14, 4, 20, 651, DateTimeKind.Local).AddTicks(5729));

            migrationBuilder.UpdateData(
                table: "PromptAiSettings",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "SystemPreamble", "UpdatedAt" },
                values: new object[] { "Du bist eine expert*innengeführte Assistenz für Instructional Design, Hochschuldidaktik und wissenschaftliches Arbeiten.\nSchreibe in präzisem, formalem Deutsch für Forschung & Lehre. Nutze wissenschaftliche Terminologie.\nHalte dich strikt an das JSON-Schema (Structured Outputs). Antworte ausschließlich mit JSON.\nJedes Filter-Item muss eine überprüfbare Leistung erzeugen (Artefakt/Metrik/Kriterium, Quellenstandard).", new DateTime(2025, 11, 10, 13, 4, 20, 651, DateTimeKind.Utc).AddTicks(5164) });

            migrationBuilder.UpdateData(
                table: "PromptTypeGuidances",
                keyColumn: "Id",
                keyValue: 1,
                column: "UpdatedAt",
                value: new DateTime(2025, 11, 10, 13, 4, 20, 651, DateTimeKind.Utc).AddTicks(5192));

            migrationBuilder.UpdateData(
                table: "PromptTypeGuidances",
                keyColumn: "Id",
                keyValue: 2,
                column: "UpdatedAt",
                value: new DateTime(2025, 11, 10, 13, 4, 20, 651, DateTimeKind.Utc).AddTicks(5194));

            migrationBuilder.UpdateData(
                table: "PromptTypeGuidances",
                keyColumn: "Id",
                keyValue: 3,
                column: "UpdatedAt",
                value: new DateTime(2025, 11, 10, 13, 4, 20, 651, DateTimeKind.Utc).AddTicks(5194));

            migrationBuilder.UpdateData(
                table: "PromptTypeGuidances",
                keyColumn: "Id",
                keyValue: 4,
                column: "UpdatedAt",
                value: new DateTime(2025, 11, 10, 13, 4, 20, 651, DateTimeKind.Utc).AddTicks(5195));

            migrationBuilder.UpdateData(
                table: "PromptTypeGuidances",
                keyColumn: "Id",
                keyValue: 5,
                column: "UpdatedAt",
                value: new DateTime(2025, 11, 10, 13, 4, 20, 651, DateTimeKind.Utc).AddTicks(5196));

            migrationBuilder.UpdateData(
                table: "PromptTypeGuidances",
                keyColumn: "Id",
                keyValue: 6,
                column: "UpdatedAt",
                value: new DateTime(2025, 11, 10, 13, 4, 20, 651, DateTimeKind.Utc).AddTicks(5197));

            migrationBuilder.UpdateData(
                table: "PromptTypeGuidances",
                keyColumn: "Id",
                keyValue: 7,
                column: "UpdatedAt",
                value: new DateTime(2025, 11, 10, 13, 4, 20, 651, DateTimeKind.Utc).AddTicks(5198));

            migrationBuilder.UpdateData(
                table: "Skills",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 10, 14, 4, 20, 651, DateTimeKind.Local).AddTicks(5480));

            migrationBuilder.CreateIndex(
                name: "IX_GlobalPromptConfigTypeGuidances_GlobalPromptConfigId",
                table: "GlobalPromptConfigTypeGuidances",
                column: "GlobalPromptConfigId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "GlobalPromptConfigTypeGuidances");

            migrationBuilder.DropTable(
                name: "GlobalPromptConfigs");

            migrationBuilder.UpdateData(
                table: "ContactMessageSettings",
                keyColumn: "Id",
                keyValue: 1,
                column: "Message",
                value: "Um unser Prompt-Engineering-Tool auszuprobieren,\r\nkontaktieren Sie uns bitte per E-Mail.");

            migrationBuilder.UpdateData(
                table: "PortalVideo",
                keyColumn: "Id",
                keyValue: 1,
                column: "UploadDate",
                value: new DateTime(2025, 11, 10, 10, 52, 39, 487, DateTimeKind.Local).AddTicks(3193));

            migrationBuilder.UpdateData(
                table: "PromptAiSettings",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "SystemPreamble", "UpdatedAt" },
                values: new object[] { "Du bist eine expert*innengeführte Assistenz für Instructional Design, Hochschuldidaktik und wissenschaftliches Arbeiten.\r\nSchreibe in präzisem, formalem Deutsch für Forschung & Lehre. Nutze wissenschaftliche Terminologie.\r\nHalte dich strikt an das JSON-Schema (Structured Outputs). Antworte ausschließlich mit JSON.\r\nJedes Filter-Item muss eine überprüfbare Leistung erzeugen (Artefakt/Metrik/Kriterium, Quellenstandard).", new DateTime(2025, 11, 10, 9, 52, 39, 487, DateTimeKind.Utc).AddTicks(2718) });

            migrationBuilder.UpdateData(
                table: "PromptTypeGuidances",
                keyColumn: "Id",
                keyValue: 1,
                column: "UpdatedAt",
                value: new DateTime(2025, 11, 10, 9, 52, 39, 487, DateTimeKind.Utc).AddTicks(2743));

            migrationBuilder.UpdateData(
                table: "PromptTypeGuidances",
                keyColumn: "Id",
                keyValue: 2,
                column: "UpdatedAt",
                value: new DateTime(2025, 11, 10, 9, 52, 39, 487, DateTimeKind.Utc).AddTicks(2745));

            migrationBuilder.UpdateData(
                table: "PromptTypeGuidances",
                keyColumn: "Id",
                keyValue: 3,
                column: "UpdatedAt",
                value: new DateTime(2025, 11, 10, 9, 52, 39, 487, DateTimeKind.Utc).AddTicks(2745));

            migrationBuilder.UpdateData(
                table: "PromptTypeGuidances",
                keyColumn: "Id",
                keyValue: 4,
                column: "UpdatedAt",
                value: new DateTime(2025, 11, 10, 9, 52, 39, 487, DateTimeKind.Utc).AddTicks(2746));

            migrationBuilder.UpdateData(
                table: "PromptTypeGuidances",
                keyColumn: "Id",
                keyValue: 5,
                column: "UpdatedAt",
                value: new DateTime(2025, 11, 10, 9, 52, 39, 487, DateTimeKind.Utc).AddTicks(2747));

            migrationBuilder.UpdateData(
                table: "PromptTypeGuidances",
                keyColumn: "Id",
                keyValue: 6,
                column: "UpdatedAt",
                value: new DateTime(2025, 11, 10, 9, 52, 39, 487, DateTimeKind.Utc).AddTicks(2748));

            migrationBuilder.UpdateData(
                table: "PromptTypeGuidances",
                keyColumn: "Id",
                keyValue: 7,
                column: "UpdatedAt",
                value: new DateTime(2025, 11, 10, 9, 52, 39, 487, DateTimeKind.Utc).AddTicks(2748));

            migrationBuilder.UpdateData(
                table: "Skills",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 10, 10, 52, 39, 487, DateTimeKind.Local).AddTicks(2968));
        }
    }
}
