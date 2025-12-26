using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace ProjectsWebApp.DataAccsess.Migrations
{
    /// <inheritdoc />
    public partial class NewVersionMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "MakerSpaceProjects",
                keyColumn: "Id",
                keyValue: 1001);

            migrationBuilder.DeleteData(
                table: "MakerSpaceProjects",
                keyColumn: "Id",
                keyValue: 1002);

            migrationBuilder.DeleteData(
                table: "MakerSpaceProjects",
                keyColumn: "Id",
                keyValue: 1008);

            migrationBuilder.DeleteData(
                table: "MakerSpaceProjects",
                keyColumn: "Id",
                keyValue: 1009);

            migrationBuilder.DeleteData(
                table: "MakerSpaceProjects",
                keyColumn: "Id",
                keyValue: 1010);

            migrationBuilder.DeleteData(
                table: "MakerSpaceProjects",
                keyColumn: "Id",
                keyValue: 1011);

            migrationBuilder.DeleteData(
                table: "MakerSpaceProjects",
                keyColumn: "Id",
                keyValue: 1012);

            migrationBuilder.DeleteData(
                table: "MakerSpaceProjects",
                keyColumn: "Id",
                keyValue: 1013);

            migrationBuilder.DeleteData(
                table: "MakerSpaceProjects",
                keyColumn: "Id",
                keyValue: 1014);

            migrationBuilder.DeleteData(
                table: "MakerSpaceProjects",
                keyColumn: "Id",
                keyValue: 1015);

            migrationBuilder.DeleteData(
                table: "MakerSpaceProjects",
                keyColumn: "Id",
                keyValue: 1018);

            migrationBuilder.DeleteData(
                table: "MakerSpaceProjects",
                keyColumn: "Id",
                keyValue: 1019);

            migrationBuilder.DeleteData(
                table: "MakerSpaceProjects",
                keyColumn: "Id",
                keyValue: 1020);

            migrationBuilder.DeleteData(
                table: "MakerSpaceProjects",
                keyColumn: "Id",
                keyValue: 1021);

            migrationBuilder.DeleteData(
                table: "MakerSpaceProjects",
                keyColumn: "Id",
                keyValue: 1022);

            migrationBuilder.DeleteData(
                table: "MakerSpaceProjects",
                keyColumn: "Id",
                keyValue: 1023);

            migrationBuilder.DeleteData(
                table: "MakerSpaceProjects",
                keyColumn: "Id",
                keyValue: 1024);

            migrationBuilder.DeleteData(
                table: "MakerSpaceProjects",
                keyColumn: "Id",
                keyValue: 1025);

            migrationBuilder.DeleteData(
                table: "MakerSpaceProjects",
                keyColumn: "Id",
                keyValue: 1026);

            migrationBuilder.DeleteData(
                table: "MakerSpaceProjects",
                keyColumn: "Id",
                keyValue: 1027);

            migrationBuilder.DeleteData(
                table: "MakerSpaceProjects",
                keyColumn: "Id",
                keyValue: 1030);

            migrationBuilder.DeleteData(
                table: "MakerSpaceProjects",
                keyColumn: "Id",
                keyValue: 1031);

            migrationBuilder.DeleteData(
                table: "MakerSpaceProjects",
                keyColumn: "Id",
                keyValue: 1032);

            migrationBuilder.DeleteData(
                table: "MakerSpaceProjects",
                keyColumn: "Id",
                keyValue: 1033);

            migrationBuilder.DeleteData(
                table: "MakerSpaceProjects",
                keyColumn: "Id",
                keyValue: 1034);

            migrationBuilder.DeleteData(
                table: "MakerSpaceProjects",
                keyColumn: "Id",
                keyValue: 1035);

            migrationBuilder.DeleteData(
                table: "MakerSpaceProjects",
                keyColumn: "Id",
                keyValue: 1036);

            migrationBuilder.DeleteData(
                table: "MakerSpaceProjects",
                keyColumn: "Id",
                keyValue: 1037);

            migrationBuilder.DeleteData(
                table: "MakerSpaceProjects",
                keyColumn: "Id",
                keyValue: 1038);

            migrationBuilder.DeleteData(
                table: "MakerSpaceProjects",
                keyColumn: "Id",
                keyValue: 1039);

            migrationBuilder.DeleteData(
                table: "MakerSpaceProjects",
                keyColumn: "Id",
                keyValue: 1040);

            migrationBuilder.DeleteData(
                table: "MakerSpaceProjects",
                keyColumn: "Id",
                keyValue: 1041);

            migrationBuilder.DeleteData(
                table: "MakerSpaceProjects",
                keyColumn: "Id",
                keyValue: 1042);

            migrationBuilder.DeleteData(
                table: "MakerSpaceProjects",
                keyColumn: "Id",
                keyValue: 1043);

            migrationBuilder.DeleteData(
                table: "MakerSpaceProjects",
                keyColumn: "Id",
                keyValue: 1044);

            migrationBuilder.DeleteData(
                table: "MakerSpaceProjects",
                keyColumn: "Id",
                keyValue: 1045);

            migrationBuilder.DeleteData(
                table: "MakerSpaceProjects",
                keyColumn: "Id",
                keyValue: 1046);

            migrationBuilder.DeleteData(
                table: "MakerSpaceProjects",
                keyColumn: "Id",
                keyValue: 1047);

            migrationBuilder.DeleteData(
                table: "MakerSpaceProjects",
                keyColumn: "Id",
                keyValue: 1048);

            migrationBuilder.DeleteData(
                table: "MakerSpaceProjects",
                keyColumn: "Id",
                keyValue: 1049);

            migrationBuilder.DeleteData(
                table: "MakerSpaceProjects",
                keyColumn: "Id",
                keyValue: 1050);

            migrationBuilder.AddColumn<string>(
                name: "FilterSystemPreamble",
                table: "PromptAiSettings",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "KiAssistantSystemPrompt",
                table: "PromptAiSettings",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "SmartSelectionSystemPreamble",
                table: "PromptAiSettings",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "SmartSelectionUserPrompt",
                table: "PromptAiSettings",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "Assistants",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true),
                    AvatarUrl = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: true),
                    SystemPrompt = table.Column<string>(type: "text", nullable: false),
                    Goals = table.Column<string>(type: "character varying(2048)", maxLength: 2048, nullable: true),
                    Licenses = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: true),
                    AuthorName = table.Column<string>(type: "text", nullable: true),
                    CreatedByUserId = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    IsGlobal = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Assistants", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SemanticIndexEntries",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    EntityType = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    EntityId = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    Title = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    Content = table.Column<string>(type: "text", nullable: true),
                    EmbeddingJson = table.Column<string>(type: "text", nullable: true),
                    VectorNorm = table.Column<double>(type: "double precision", nullable: false),
                    Group = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    OwnerUserId = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SemanticIndexEntries", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AssistantEmbeddings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    AssistantId = table.Column<int>(type: "integer", nullable: false),
                    SourceFileName = table.Column<string>(type: "character varying(260)", maxLength: 260, nullable: true),
                    Content = table.Column<string>(type: "text", nullable: false),
                    EmbeddingJson = table.Column<string>(type: "text", nullable: false),
                    UploadedAtUtc = table.Column<DateTime>(type: "timestamp without time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AssistantEmbeddings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AssistantEmbeddings_Assistants_AssistantId",
                        column: x => x.AssistantId,
                        principalTable: "Assistants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.UpdateData(
                table: "Events",
                keyColumn: "Id",
                keyValue: 1,
                column: "Termin",
                value: new DateTime(2025, 11, 4, 0, 0, 0, 0, DateTimeKind.Local));

            migrationBuilder.UpdateData(
                table: "MakerSpaceProjects",
                keyColumn: "Id",
                keyValue: 6,
                columns: new[] { "ImageUrl", "Tags" },
                values: new object[] { "/images/makerspace/848e2db6-9c84-4da1-b336-7c357fed7204.svg", "KI, Bilder" });

            migrationBuilder.UpdateData(
                table: "MakerSpaceProjects",
                keyColumn: "Id",
                keyValue: 12,
                column: "Tags",
                value: "KI, Color");

            migrationBuilder.UpdateData(
                table: "MakerSpaceProjects",
                keyColumn: "Id",
                keyValue: 16,
                column: "Tags",
                value: "KI, Forschung");

            migrationBuilder.UpdateData(
                table: "MakerSpaceProjects",
                keyColumn: "Id",
                keyValue: 22,
                column: "Tags",
                value: "KI, Schriften");

            migrationBuilder.UpdateData(
                table: "MakerSpaceProjects",
                keyColumn: "Id",
                keyValue: 25,
                column: "Tags",
                value: "KI, Visualisieren, Design ");

            migrationBuilder.UpdateData(
                table: "MakerSpaceProjects",
                keyColumn: "Id",
                keyValue: 31,
                column: "Tags",
                value: "KI, Color, Design");

            migrationBuilder.UpdateData(
                table: "MakerSpaceProjects",
                keyColumn: "Id",
                keyValue: 32,
                columns: new[] { "ImageUrl", "Tags" },
                values: new object[] { "/images/makerspace/d589d7a5-62c1-47df-9934-a6d44a4027e7.svg", "KI, Programmieren " });

            migrationBuilder.UpdateData(
                table: "MakerSpaceProjects",
                keyColumn: "Id",
                keyValue: 33,
                column: "Tags",
                value: "KI, H5P");

            migrationBuilder.UpdateData(
                table: "MakerSpaceProjects",
                keyColumn: "Id",
                keyValue: 36,
                column: "Tags",
                value: "KI, Forschung ");

            migrationBuilder.UpdateData(
                table: "MakerSpaceProjects",
                keyColumn: "Id",
                keyValue: 39,
                column: "Tags",
                value: "KI, Forschung ");

            migrationBuilder.UpdateData(
                table: "MakerSpaceProjects",
                keyColumn: "Id",
                keyValue: 40,
                columns: new[] { "ImageUrl", "Tags" },
                values: new object[] { "https://cdn.prod.website-files.com/65e9bdf0fae79e05e213200c/660fe39debd7cc7092e09609_Untitled%20design.webp", "KI, Lehre planen" });

            migrationBuilder.UpdateData(
                table: "MakerSpaceProjects",
                keyColumn: "Id",
                keyValue: 43,
                columns: new[] { "ImageUrl", "Tags" },
                values: new object[] { "/images/makerspace/2583cec8-494d-4271-bf21-67dd3f24b6c9.png", "KI, Bilder " });

            migrationBuilder.UpdateData(
                table: "MakerSpaceProjects",
                keyColumn: "Id",
                keyValue: 45,
                column: "Tags",
                value: "KI, Übersetzungen ");

            migrationBuilder.UpdateData(
                table: "MakerSpaceProjects",
                keyColumn: "Id",
                keyValue: 48,
                column: "Tags",
                value: "KI, Visualisieren");

            migrationBuilder.UpdateData(
                table: "MakerSpaceProjects",
                keyColumn: "Id",
                keyValue: 50,
                column: "Tags",
                value: "KI, Audio, Sound, Video");

            migrationBuilder.UpdateData(
                table: "MakerSpaceProjects",
                keyColumn: "Id",
                keyValue: 51,
                column: "Tags",
                value: "KI, Plagiatprüfung");

            migrationBuilder.UpdateData(
                table: "MakerSpaceProjects",
                keyColumn: "Id",
                keyValue: 52,
                columns: new[] { "ImageUrl", "Tags" },
                values: new object[] { "/images/makerspace/f2a76865-9e1f-49dc-8acb-83bc6574b0f0.svg", "KI, Bilder, Video, Musik, 3D-Objekte" });

            migrationBuilder.UpdateData(
                table: "MakerSpaceProjects",
                keyColumn: "Id",
                keyValue: 53,
                columns: new[] { "ImageUrl", "Tags" },
                values: new object[] { "/images/makerspace/e0642455-875c-41d5-a201-afb3a4ed6599.svg", "KI, Programmieren" });

            migrationBuilder.UpdateData(
                table: "MakerSpaceProjects",
                keyColumn: "Id",
                keyValue: 54,
                columns: new[] { "Description", "ImageUrl", "Tags" },
                values: new object[] { "<p>Die App ist eine KI-gestützte Plattform, die Bilder in traumhafte, surreal-verzerrte Kunstwerke verwandelt, indem sie Muster und Strukturen verstärkt.</p>", "/images/makerspace/61d3aa04-0d94-43eb-bc9f-2e994a24b3dc.svg", "KI, Bilder" });

            migrationBuilder.UpdateData(
                table: "MakerSpaceProjects",
                keyColumn: "Id",
                keyValue: 55,
                column: "Tags",
                value: "KI, Textanalyse, Schreibprozesse ");

            migrationBuilder.UpdateData(
                table: "MakerSpaceProjects",
                keyColumn: "Id",
                keyValue: 56,
                column: "Tags",
                value: "KI, Bilder");

            migrationBuilder.UpdateData(
                table: "MakerSpaceProjects",
                keyColumn: "Id",
                keyValue: 57,
                column: "Tags",
                value: "KI, Forschung ");

            migrationBuilder.UpdateData(
                table: "MakerSpaceProjects",
                keyColumn: "Id",
                keyValue: 58,
                columns: new[] { "ImageUrl", "Tags" },
                values: new object[] { "/images/makerspace/d3dd6746-3d32-44b7-b04c-63ea73f9eb00.webp", "KI, Sound" });

            migrationBuilder.UpdateData(
                table: "MakerSpaceProjects",
                keyColumn: "Id",
                keyValue: 59,
                column: "Tags",
                value: "KI, Plagiatprüfung");

            migrationBuilder.UpdateData(
                table: "MakerSpaceProjects",
                keyColumn: "Id",
                keyValue: 60,
                column: "Tags",
                value: "KI, Sprachgeneratoren");

            migrationBuilder.UpdateData(
                table: "MakerSpaceProjects",
                keyColumn: "Id",
                keyValue: 61,
                column: "Tags",
                value: "KI, Forschung, Zitationen");

            migrationBuilder.UpdateData(
                table: "MakerSpaceProjects",
                keyColumn: "Id",
                keyValue: 62,
                column: "Tags",
                value: "KI, Forschung");

            migrationBuilder.UpdateData(
                table: "MakerSpaceProjects",
                keyColumn: "Id",
                keyValue: 63,
                column: "Tags",
                value: "KI, Forschung, Literaturrecherche ");

            migrationBuilder.UpdateData(
                table: "MakerSpaceProjects",
                keyColumn: "Id",
                keyValue: 64,
                column: "Tags",
                value: "KI, Visualisieren");

            migrationBuilder.UpdateData(
                table: "MakerSpaceProjects",
                keyColumn: "Id",
                keyValue: 66,
                column: "Tags",
                value: "KI, Schreibprozesse ");

            migrationBuilder.UpdateData(
                table: "MakerSpaceProjects",
                keyColumn: "Id",
                keyValue: 67,
                column: "Tags",
                value: "KI, Forschung, Literaturrecherche ");

            migrationBuilder.UpdateData(
                table: "MakerSpaceProjects",
                keyColumn: "Id",
                keyValue: 69,
                columns: new[] { "Tags", "Top" },
                values: new object[] { "KI, Video", false });

            migrationBuilder.UpdateData(
                table: "MakerSpaceProjects",
                keyColumn: "Id",
                keyValue: 70,
                columns: new[] { "Tags", "Top" },
                values: new object[] { "KI, Design", false });

            migrationBuilder.UpdateData(
                table: "MakerSpaceProjects",
                keyColumn: "Id",
                keyValue: 73,
                columns: new[] { "ImageUrl", "Tags" },
                values: new object[] { "/images/makerspace/c0660b95-e69c-4c1e-8e99-df19fb7ba438.png", "KI, Prompt-Repository, Bilder " });

            migrationBuilder.UpdateData(
                table: "MakerSpaceProjects",
                keyColumn: "Id",
                keyValue: 74,
                columns: new[] { "Description", "ImageUrl", "Tags" },
                values: new object[] { "<p>Immersity AI verwandelt mit KI 2D-Bilder und -Videos in fbewegende 3D-Erlebnisse. Nutzer können ihre Inhalte in 3D-Motion-Bilder, 3D-Videos oder 3D-Bilder umwandeln und sie auf XR-Geräten wie Apple Vision Pro und Meta Quest erleben.</p>", "/images/makerspace/a598d75d-c36b-4110-b1cd-1ab9fabb3d26.svg", "KI, Video " });

            migrationBuilder.UpdateData(
                table: "MakerSpaceProjects",
                keyColumn: "Id",
                keyValue: 75,
                column: "Tags",
                value: "KI, Quiz, Prüfungen, Feedback");

            migrationBuilder.UpdateData(
                table: "MakerSpaceProjects",
                keyColumn: "Id",
                keyValue: 77,
                column: "Tags",
                value: "KI, Textanalyse, Schreibprozesse ");

            migrationBuilder.UpdateData(
                table: "MakerSpaceProjects",
                keyColumn: "Id",
                keyValue: 79,
                columns: new[] { "Tags", "events" },
                values: new object[] { "KI, Logo, Design", false });

            migrationBuilder.UpdateData(
                table: "MakerSpaceProjects",
                keyColumn: "Id",
                keyValue: 80,
                columns: new[] { "ImageUrl", "Tags" },
                values: new object[] { "/images/makerspace/7584bd6a-59c5-4b1f-a0a2-ff39c6c928c6.svg", "KI, Bilder " });

            migrationBuilder.UpdateData(
                table: "MakerSpaceProjects",
                keyColumn: "Id",
                keyValue: 81,
                column: "Tags",
                value: "KI, Literaturrecherche, Textanalyse ");

            migrationBuilder.UpdateData(
                table: "MakerSpaceProjects",
                keyColumn: "Id",
                keyValue: 83,
                column: "Tags",
                value: "KI, Forschung, Textanalyse ");

            migrationBuilder.UpdateData(
                table: "MakerSpaceProjects",
                keyColumn: "Id",
                keyValue: 85,
                column: "Tags",
                value: "KI, Forschung ");

            migrationBuilder.UpdateData(
                table: "MakerSpaceProjects",
                keyColumn: "Id",
                keyValue: 87,
                columns: new[] { "Tags", "events" },
                values: new object[] { "KI, Bilder ", false });

            migrationBuilder.UpdateData(
                table: "MakerSpaceProjects",
                keyColumn: "Id",
                keyValue: 88,
                column: "Tags",
                value: "KI, Visualisieren");

            migrationBuilder.UpdateData(
                table: "MakerSpaceProjects",
                keyColumn: "Id",
                keyValue: 91,
                column: "Tags",
                value: "KI, Visualisieren, Design ");

            migrationBuilder.UpdateData(
                table: "MakerSpaceProjects",
                keyColumn: "Id",
                keyValue: 92,
                column: "Tags",
                value: "KI, Sprachgeneratoren");

            migrationBuilder.UpdateData(
                table: "MakerSpaceProjects",
                keyColumn: "Id",
                keyValue: 94,
                column: "Tags",
                value: "KI, Visualisieren");

            migrationBuilder.UpdateData(
                table: "MakerSpaceProjects",
                keyColumn: "Id",
                keyValue: 95,
                column: "Tags",
                value: "KI, Schreibprozesse");

            migrationBuilder.UpdateData(
                table: "MakerSpaceProjects",
                keyColumn: "Id",
                keyValue: 97,
                column: "Tags",
                value: "KI, Transkription");

            migrationBuilder.UpdateData(
                table: "MakerSpaceProjects",
                keyColumn: "Id",
                keyValue: 98,
                columns: new[] { "Forschung", "Tags", "events" },
                values: new object[] { false, "KI, Literaturrecherche, Textanalyse", false });

            migrationBuilder.UpdateData(
                table: "MakerSpaceProjects",
                keyColumn: "Id",
                keyValue: 99,
                columns: new[] { "Tags", "events" },
                values: new object[] { "KI, Bilder", false });

            migrationBuilder.UpdateData(
                table: "MakerSpaceProjects",
                keyColumn: "Id",
                keyValue: 102,
                column: "Tags",
                value: "KI, Schreibprozesse, Plagiatprüfung");

            migrationBuilder.UpdateData(
                table: "MakerSpaceProjects",
                keyColumn: "Id",
                keyValue: 103,
                columns: new[] { "ImageUrl", "Tags" },
                values: new object[] { null, "KI, Prüfungen, Quiz" });

            migrationBuilder.UpdateData(
                table: "MakerSpaceProjects",
                keyColumn: "Id",
                keyValue: 115,
                column: "Tags",
                value: "KI, Schreibassistent");

            migrationBuilder.InsertData(
                table: "MakerSpaceProjects",
                columns: new[] { "Id", "Beitraege", "Description", "DisplayOrder", "Forschung", "ITRecht", "ImageUrl", "ProjectUrl", "Tags", "Title", "Top", "download", "events", "lesezeichen", "netzwerk", "tutorial" },
                values: new object[,]
                {
                    { 1, false, "A low-cost 3D printed prosthetic hand designed for children. Fully open-source and customizable.", 0, false, false, null, "https://example.com/prosthetic-hand", "3D Printing, Prosthetics, Open Source", "3D Printed Prosthetic Hand", false, false, false, false, false, false },
                    { 2, false, "An automatic watering system for plants using soil moisture sensors and Arduino.", 0, false, false, null, "https://example.com/smart-watering", "IoT, Arduino, Sensors, Plants", "Smart Plant Watering System", false, false, false, false, false, false },
                    { 3, false, "Build your own CNC milling machine using affordable components and open hardware designs.", 0, false, false, null, "https://example.com/desktop-cnc", "CNC, DIY, Fabrication, Open Hardware", "DIY Desktop CNC Machine", false, false, false, false, false, false },
                    { 4, false, "A smart assistant using Raspberry Pi and voice recognition libraries to execute custom commands.", 0, false, false, null, "https://example.com/voice-pi", "Raspberry Pi, Voice Recognition, Python, AI", "Voice-Controlled Assistant with Raspberry Pi", false, false, false, false, false, false },
                    { 5, false, "<p>Dieser Chat-Assistent-Workshop bietet Ihnen einen ersten Einstieg, um die grundlegenden Möglichkeiten von KI-gesteuerten Assistenten zu entdecken. Gemeinsam werden wir im Workshop die Funktionen und Potenziale dieser Technologie kennenlernen und erste praktische Erfahrungen sammeln. Um aktiv an der Übung mit dem Edu-Assistenten teilzunehmen und dessen Funktionen zu erkunden, benötigen Sie ein Hugging Face-Konto.</p>", 5, false, false, "/images/makerspace/e7e326fa-0eb6-45c1-b76a-fbe2cf0ff7a8.jpg", "https://huggingface.co/chat/assistant/679f38d80b57a15c9d556ba5", "KI, Chat-Assistent", "Edu-Coach-Workshop", true, false, false, false, false, false },
                    { 7, false, "<p>Der H5P-Prüfungsassistent unterstützt die Erstellung von Prüfungsaufgaben: Mit dem H5P-Tool unterstützt er Lehrende effizient bei der Generierung von Multiple-Choice, Single-Choice und weiteren Aufgabentypen.</p>", 7, false, false, "/images/makerspace/c88ad8cd-97a4-47a3-9131-dba5eea63c39.jpg", "https://huggingface.co/chat/assistant/67b2092f40b8f74a387e3878", "KI, Chat-Assistent", "H5P Prüfungsassistent", true, false, false, false, false, false },
                    { 8, false, "<p>Erleben Sie mit 3DVista die Zukunft der virtuellen Rundgänge: Erstellen Sie beeindruckende 360°-Erlebnisse mit interaktiven Hotspots, dynamischen Panoramen und anpassbaren HDR-Effekten. Tauchen Sie ein in immersive 3D-Welten für Messen, Schulungen und mehr.</p>", 8, false, false, "/images/makerspace/677a76fe-82dd-47ec-8c50-dda3af19a42b.png", "https://www.3dvista.com/de/", "360Grad, 3D-Objekte", "3D Vista", false, false, false, false, false, false },
                    { 9, false, "<p>Entdecken Sie die Vielfalt der H5P-Inhaltstypen und tauchen Sie ein in eine Welt interaktiver Lernmöglichkeiten.</p>", 9, false, false, "/images/makerspace/11a00195-7317-4277-8a1e-fcd3143ad018.png", "https://deutsch-lernen.zum.de/wiki/Kategorie:H5P-Inhaltstypen", "H5P", "ZUM - Inhaltstypen", false, false, false, false, false, false },
                    { 10, false, "<p>Zusammenstellung mit vielen Beispielen zu fast allen H5P Inhaltstypen.</p>", 10, false, false, "/images/makerspace/4c3143c0-7cfc-4d0c-8e06-a756f8437e86.png", "https://zebis.digital/start/67D3PW", "H5P", "Zedis.digital-H5P", false, false, false, false, false, false },
                    { 11, false, "<p>Tauchen Sie ein in die Welt der Virtual Reality mit Unity und der XR API. Von VR-Spielen bis hin zu interaktiven Simulationen ist alles möglich. Erfahren Sie, wie Sie mit fundierten Kenntnissen in Unity, C# und Game-Design beeindruckende VR-Inhalte für verschiedene Hardware erstellen können.</p>", 11, false, false, "/images/makerspace/284b8494-1d39-4485-bf91-e5e1bb03409b.png", "https://unity.com/de", "Unity, 3D-Objekte, 3D-Räume", "Unity", false, false, false, false, false, false },
                    { 13, false, "<p>Mit ThingLink verwandeln Sie statische Bilder und Videos in interaktive Erlebnisse - dank zahlreicher Vorlagen und Layouts, die individuell angepasst werden können. Entdecken Sie die Möglichkeiten, Ihre Medieninhalte auf ein neues Level zu heben.</p>", 13, false, false, "/images/makerspace/94781c6d-d78c-482c-9d76-4b339b348e75.svg", "https://www.thinglink.com/", "360Grad", "ThingLink", false, false, false, false, false, false },
                    { 14, false, "<p>Multimediale Elemente, automatisierte Rückmeldungen und individuelle Interaktionen für ein maßgeschneidertes Lernerlebnis. Formatives Feedback ermöglicht eine kontinuierliche Anpassung des Lernprozesses.</p>", 14, false, false, "/images/makerspace/2efe06f3-ef59-4be4-8ff2-522bc6e26993.svg", "https://www.bycs.de/themenkomplex/lernplattform/h5p/index.html", "H5P", "Themenkomplex H5P", false, false, false, false, false, false },
                    { 18, false, "<p>Entdecken Sie die innovative Marzipano Web-Applikation: Erstellen Sie beeindruckende 360°-Panorama-Szenen und -Touren mit interaktiven Links und Anmerkungen, exportieren Sie Ihr Projekt als Website und teilen Sie es auf jedem Webbrowser.</p>", 18, false, false, "/images/makerspace/0d03c422-11ed-4ea4-9bae-c3ad3ff2c7ca.png", "https://www.marzipano.net/", "360Grad", "Marzipano", false, false, false, false, false, false },
                    { 19, false, "<p>Mit H5P können Lehrkräfte in Moodle mühelos interaktive, multimediale Lerninhalte erstellen und so den Unterricht spannender und effektiver gestalten. Die Open-Source-Erweiterung fördert durch intuitive Vorlagen und automatisierte Rückmeldungen den OER-Gedanken und die Gamification im Bildungsbereich.</p>", 19, false, false, "/images/makerspace/b7096887-64f1-48aa-a01f-520fc92eb7e8.svg", "https://lehrerfortbildung-bw.de/st_digital/moodle/02_anleitungen/03trainer/03aktivitaeten/11h5p/", "H5P", "Lehrerfortbildung-bw", false, false, false, false, false, false },
                    { 20, false, "<p>Lumi H5P Desktop Editor: Erstellen und bearbeiten Sie H5P-Inhalte ganz einfach offline. Verabschieden Sie sich von der Internet-Abhängigkeit mit diesem leistungsstarken und kostenlosen Tool.</p>", 20, false, false, "/images/makerspace/f21b03c0-59f8-439d-ad07-cd7a4f741785.svg", "https://lumi.education/en/lumi-h5p-offline-desktop-editor/", "H5P", "Lumi H5P Desktop Editor", false, false, false, false, false, false },
                    { 21, false, "<p>Entdecken Sie, wie digitale Medien und das H5P-Framework das Lehren und Lernen revolutionieren können. Erfahren Sie, wie interaktive Inhalte die Wissensvermittlung effektiver gestalten und mehr Flexibilität in der Bildung ermöglichen.</p>", 21, false, false, "/images/makerspace/835949a2-6643-485a-8bc2-4315efa4a4d1.png", "https://lern-app-kompass.de/h5p-interaktive-book/", "H5P", "Lern App Kompass-H5P", false, false, false, false, false, false },
                    { 23, false, "<p>Mit H5P gestalten Sie interaktive Lerninhalte im Handumdrehen - unsere Tutorials und Praxisbeispiele zeigen Ihnen, wie es geht. Tauchen Sie ein in die Welt des digitalen Unterrichts und entdecken Sie vielseitige Anwendungsmöglichkeiten!</p>", 23, false, false, "/images/makerspace/2419c1b1-6713-41ff-98e1-2d63b65ec974.png", "https://www.media-data.org/h5p-tutorials/", "H5P", "H5P Tutorials", true, false, false, false, false, false },
                    { 24, false, "<p>Das eCampusOntario H5P Studio erlaubt interaktive Lerninhalte aus verschiedenen Fachgebieten zu erstellen, zu teilen und zu entdecken. Der Katalog bietet zahlreiche H5P-Aktivitäten, meist unter Creative Commons-Lizenzen. Als gemeinnütziges Kompetenzzentrum unterstützt eCampusOntario das technologiegestützte Lehren und Lernen.</p>", 24, false, false, "/images/makerspace/1a32e23c-8e1b-47e1-a1ba-84926f1a707a.png", "https://h5pstudio.ecampusontario.ca/", "H5P", "H5P Studio", true, false, false, false, false, false },
                    { 26, false, "<p>Entdecken Sie die vielfältigen Möglichkeiten von H5P-Inhalten im Unterricht. Von Multiple-Choice-Quizzen bis hin zu Lückentexten - erfahren Sie, wie diese interaktiven Tools das Verständnis prüfen und das Lernen bereichern. Tauchen Sie ein in die Welt von H5P und entdecken Sie, wie diese innovativen Aktivitäten den Unterricht revolutionieren können.</p>", 26, false, false, "/images/makerspace/281ca711-5a99-4789-9c2f-f2966ce86059.svg", "https://angebote.smz-stuttgart.de/pluginfile.php/803/mod_resource/content/1/H5P%20ChatGPT%20Prompts.pdf", "H5P", "H5P-SMZ-Stuttgart", true, false, false, false, false, false },
                    { 27, false, "<p>Mit Chat GPT und H5P interaktive Inhalte erstellen: Entdecken Sie, wie künstliche Intelligenz und Open-Source-Technologie zusammenkommen, um Quiz und dynamische Zusammenfassungen zu revolutionieren.</p>", 27, false, false, "/images/makerspace/1f22a05f-25ad-44e0-a203-1ed9f9718941.svg", "https://h5p.org/GPT", "KI, Prompt, H5P", "H5P-Prompt", true, false, false, false, false, false },
                    { 28, false, "<p>Ein Chatbot von OpenAI, der menschenähnliche Gespräche führen kann. Er verwendet maschinelles Lernen, um auf Texteingaben zu reagieren und Aufgaben wie Fragen beantworten, Texte generieren oder übersetzen zu übernehmen.</p>", 28, true, false, "https://upload.wikimedia.org/wikipedia/commons/thumb/e/ef/ChatGPT-Logo.svg/330px-ChatGPT-Logo.svg.png", "https://chat.openai.com/", "KI, Schreibprozesse ", "ChatGPT", true, false, false, false, false, false },
                    { 29, false, "<p>H5P revolutioniert die Erstellung interaktiver HTML5-Inhalte: Das kostenlose, quelloffene Framework ermöglicht es, ansprechende Online-Erlebnisse direkt im Browser zu gestalten und nahtlos in CMS und LMS zu integrieren. Mit umfassender Dokumentation und einer aktiven Community bietet H5P eine einfache und flexible Lösung für Entwickler und Nutzer.</p>", 29, false, false, "/images/makerspace/f59abb78-b7d9-4508-88d8-8b495339aec6.svg", "https://h5p.org/", "H5P", "H5P-ORG", true, false, false, false, false, false },
                    { 30, false, "<p>Die Integration von KI-Tools wie ChatGPT in die Erstellung von H5P-Frageformaten revolutioniert die Gestaltung interaktiver Lerninhalte. Doch nur durch präzise Anweisungen können diese automatisierten Inhalte den Bildungsprozess wirklich bereichern.</p>", 30, false, false, "/images/makerspace/535c860c-6763-4152-b60f-0396374155ba.svg", "https://h5p.org/using-ai-to-create-h5p-content", "KI, H5P", "H5P & Künstliche Intelligenz", true, false, false, false, false, false },
                    { 34, false, "<p>Entdecken Sie interaktive Lernübungen auf dem Landesbildungsserver, erstellt mit dem innovativen Programmpaket H5P. Erfahren Sie mehr in unserer Einführung zu H5P.</p>", 34, false, false, "https://www.schule-bw.de/++theme++plonetheme.lbs/img/lbs-logo-neu2.png", "https://www.schule-bw.de/themen-und-impulse/medienbildung/interaktiv/index.html", "H5P", "H5P Interaktive Übungen", true, false, false, false, false, false },
                    { 35, false, "<p>Die Software, die interaktive Inhalte zum Leben erweckt. Von interaktiven Videos bis hin zu 360°-Touren - mit H5P ist alles möglich. Tauchen Sie ein in die Welt der kreativen Möglichkeiten!</p>", 35, false, false, "/images/makerspace/fce863ac-ff40-4a55-94b8-19f310cc6215.jpg", "https://www.xn--martina-rter-llb.de/lernen-elearing/h5p-elementtypen-mit-lumi-offline-erstellen/", "H5P", "H5P-Elementtypen", true, false, false, false, false, false },
                    { 38, false, "<p>Verschiedene H5P-basierte Aufgabentypen und Beispiele im Überblick.</p>", 38, false, false, "/images/makerspace/18f92eb7-20d4-4296-a8fe-804b30590701.png", "https://www.digitale-lernumgebung.de/h5p.html", "H5P", "DiLerH5P", true, false, false, false, false, false },
                    { 41, false, "<p>Die virtuelle Revolution: Mit Character Creator von Reallusion zum perfekten 3D-Charakter. Künstler, Designer und Entwickler aufgepasst - diese Software lässt keine Wünsche offen. Erfahren Sie, wie Sie mit unzähligen Anpassungsmöglichkeiten realistische Figuren zum Leben erwecken. Werfen Sie einen Blick hinter die Kulissen der digitalen Kreativität.</p>", 41, false, false, "/images/makerspace/1b4b62cb-e363-499d-934f-f3cb05b41e2d.svg", "https://www.reallusion.com/de/character-creator/digital-human-skin-morph.html", "Avatare, 3D-Objekte, Game", "Character Creator", false, false, false, false, false, false },
                    { 42, false, "<p>Mit der Applikation Blender können 3D-Grafiken, Modelle und Animationen erstellt und in verschiedenen Medien, wie z.B. Animationsfilmen und Spielen, eingebunden werden.</p>", 42, false, false, "https://upload.wikimedia.org/wikipedia/commons/thumb/3/3c/Logo_Blender.svg/768px-Logo_Blender.svg.png", "https://www.blender.org/", "Blender, 3D-Objekte", "Blender", false, false, false, false, false, false },
                    { 44, false, "<p>Das Kompetenzzentrum digitale Barrierefreiheit.nrw hat H5P-Inhaltstypen auf Barrierefreiheit getestet und zeigt: Kein Typ ist uneingeschränkt barrierefrei. Die detaillierten Ergebnisse bieten Lehrenden wertvolle Entscheidungshilfen.</p>", 44, false, false, "/images/makerspace/0d1fde93-41dc-43b5-9984-dd68dd1747f1.png", "https://help.itc.rwth-aachen.de/service/8d9eb2f36eea4fcaa9abd0e1ca008b22/article/80a669e8423d40fcb26d41e4377b6586/", "H5P, Barrierefreiheit", "Barrierefreiheit von H5P", true, false, false, false, false, false },
                    { 46, false, "<p>Entdecken Sie, wie H5P Lehrende, eLearning-Verantwortliche und Studierende unterstützt und welche Rolle Barrierefreiheit und Usability spielen. Es bietet einen Blick hinter die Kulissen des Kompetenzzentrums, das die Zukunft des digitalen Lernens gestaltet.</p>", 46, false, false, "/images/makerspace/9aec4e91-5b3d-470d-a74e-8e426878e096.png", "https://barrierefreiheit.dh.nrw/tests/ergebnisse/h5p", "H5P, Barrierefreiheit", "Barrierefreiheit und H5P", true, false, false, false, false, false },
                    { 47, false, "<p>Das BNE OER-Projekt bietet digitale Lerneinheiten zur Bildung für nachhaltige Entwicklung (BNE) als Open Educational Resources (OER) an. Sie unterstützen Pädagoginnen dabei, Schülerinnen für nachhaltiges Handeln zu befähigen. Die Materialien richten sich an Studierende der Frühpädagogik und angehende Lehrkräfte. Entstanden sind diese im Rahmen verschiedener Projekte, die auf der Webseite vorgestellt werden.</p>", 47, false, false, "/images/makerspace/952392da-d884-43b2-a66f-b6523929bab0.png", "https://bne-oer.de/", "BNE", "BNE OER: Bildung für nachhaltige Entwicklung", false, false, false, false, false, false },
                    { 49, false, "<p>Envato vorher unter Deblank bekannt, optimiert Kreativität: KI-Designwerkzeuge und individuelle Farbpaletten begleiten Designer.</p>", 49, false, false, "/images/makerspace/28a0e1bd-b8e9-4424-909a-84a858752ac0.svg", "https://elements.envato.com/de/", "KI, Color ", "Envato", false, false, false, false, false, false },
                    { 65, false, "<p>Ein Tool, das von GitHub in Zusammenarbeit mit OpenAI entwickelt wurde. Es fungiert als intelligenter Code-Editor, der Entwicklern hilft, Code schneller zu schreiben, indem es automatisch Code-Vorschläge basierend auf dem aktuellen Kontext im Editor macht.</p>", 65, false, false, "/images/makerspace/7c4451b2-1131-41ab-a8c5-ae61585b0fe9.png", "https://github.com/features/copilot", "KI, Programmieren ", "GitHub Copilot", true, false, false, false, false, false },
                    { 68, false, "<p>Dieses Asset-Pack enthält eine charmante Low-Poly-Waldszene mit stilisierten Bäumen, Felsen, Grasflächen und natürlichen Elementen wie Baumstümpfen und Büschen - ideal für all Ihre Projekte!</p><p>&nbsp;</p><p>Quelle: <a href=https://assetstore.unity.com/>https://assetstore.unity.com/</a></p>", 68, false, false, "/images/makerspace/8610a88c-bcfa-4ae6-817c-cbaad69d25b9.webp", "https://assetstore.unity.com/packages/3d/environments/landscapes/low-poly-simple-nature-pack-162153", "Unity", "Simple Naturgegenstände (Low-Poly)", false, false, false, false, false, false },
                    { 71, false, "<p>Mit KI-Chatbot-Modellen kannst du deinen eigenen KI-Chat-Assistenten erstellen. Diese Modelle unterstützen Textgenerierung, interaktive Gespräche und die Möglichkeit, Feedback von Nutzern zu sammeln, indem sie auf natürliche Sprache reagieren. Sie können auch für Rollenspiele, Schreibprozesse und Feedback-Sammlung genutzt werden, um dynamische Interaktionen zu fördern und kontinuierliche Verbesserungen im kreativen oder akademischen Schreiben zu ermöglichen.</p>", 71, false, false, "/images/makerspace/f5628ed1-6fbd-476e-9cc6-1fa22d80bf5f.svg", "https://huggingface.co/chat/", "KI, Feedback, Schreibprozesse", "Hugging Face Chat", false, false, false, false, false, false },
                    { 72, false, "<p>Mit AllSky Free verleihst du deinen Projekten eine realistische und atmosphärische Himmelskulisse. Das Asset-Pack umfasst zehn hochwertige Skyboxen, die unterschiedliche Tageszeiten und Wetterbedingungen darstellen - von klaren, sonnigen Mittagen bis hin zu dramatischen Sonnenuntergängen und bewölkten Himmeln.</p><p>&nbsp;</p><p>Quelle: <a href=https://assetstore.unity.com/>https://assetstore.unity.com/</a></p>", 72, false, false, "https://assetstorev1-prd-cdn.unity3d.com/key-image/04f13b36-8dda-4914-b18f-2d6f8e9fc510.webp", "https://assetstore.unity.com/packages/2d/textures-materials/sky/allsky-free-10-sky-skybox-set-146014", "Unity", "AllSky Free", false, false, false, false, false, false },
                    { 76, false, "<p>Die App Future-Skills vorher bekannt unter KI - Skills, bietet KI-gestützte Einsatzszenarien zur Förderung von Lehre, Lernen und Forschung. Sie ermöglicht die Erkundung und Weiterentwicklung innovativer Ansätze im Bereich der künstlichen Intelligenz und soll Lehrende sowie Forschende mit wertvollen Impulsen für ihre Arbeit unterstützen. Ergänzend dazu bietet eine kuratierte Linkliste weiterführende Informationen zu zentralen Themen der künstlichen Intelligenz.</p>", 76, false, false, "/images/makerspace/258be312-4e4f-4d44-b7f9-185846bc55fd.png", "https://elp-app.de/apps/future-skills/", "KI, Lehre planen, Visualisieren", "Future-Skills", true, false, false, false, false, false },
                    { 78, false, "<p>Dieses Asset-Pack enthält 8 stilisierte 3D-Tiermodelle - perfekt für Spiele, Lernwelten oder stylisierte Naturumgebungen. Zudem ist jedes Tier mit 18 vorgefertigten Animationen ausgestattet. Unter den Tieren gibt es:</p><ul><li>Vogel</li><li>Fisch</li><li>Echse</li><li>Rennmaus</li><li>Schlange</li><li>Affe</li><li>Reh</li><li>Tintenfisch</li></ul><p>&nbsp;</p><p>Quelle: <a href=https://assetstore.unity.com/>https://assetstore.unity.com/</a></p>", 78, false, false, "https://assetstorev1-prd-cdn.unity3d.com/key-image/04fb0ee1-1819-47a5-be7a-844a1d3bd2d1.webp", "https://assetstore.unity.com/packages/3d/characters/animals/quirky-series-free-animals-pack-178235", "Unity", "3D-Tier-Modelle (Low Poly)", false, false, false, false, false, false },
                    { 84, false, "<p>Dieses umfangreiche Icon-Asset-Pack bietet 925 vielseitige Symbole zur nahtlosen Integration in deine Projekte.&nbsp;</p>", 84, false, false, "https://assetstorev1-prd-cdn.unity3d.com/package-screenshot/2c15b171-4e71-4ead-bd8d-7578b9d76fd2.webp", "https://assetstore.unity.com/packages/2d/gui/flat-pack-gui-307236", "Unity", "Icons", false, false, false, false, false, false },
                    { 86, false, "<p>Dieses Asset-Pack bietet eine Sammlung von 3D-Modellen für ein Schullabor - ideal für die Darstellung von wissenschaftlichen Experimenten und Bildungseinrichtungen.</p><p>&nbsp;</p><p>Quelle: <a href=https://assetstore.unity.com/>https://assetstore.unity.com/</a></p>", 86, false, false, "https://assetstorev1-prd-cdn.unity3d.com/key-image/d3ec5cf1-89c8-460a-8ff8-e704372e5cb9.webp", "https://assetstore.unity.com/packages/3d/environments/chemistry-lab-items-pack-220212", "Unity", "3D-Schullabor", false, false, false, false, false, false },
                    { 89, false, "<p>Mit der App werden Open-Source-Modelle zur Text- und Sprachverarbeitung sowie zur Bilderstellung angeboten, die präzise Textgenerierung und Bildkreation ermöglichen.</p>", 89, false, false, "https://upload.wikimedia.org/wikipedia/de/thumb/1/1c/Mistral_AI_logo.svg/1200px-Mistral_AI_logo.svg.png?20240911082714", "https://mistral.ai/?utm_source=chatgpt.com", "KI, Schreibprozesse, Bilder ", "Mistral", true, false, false, false, false, false },
                    { 90, false, "<p>Mit diesem Asset-Pack können Sie Ihren Projekten eine lebendige Lernumgebung verleihen. Das Arbeitszimmer ist im altmodischen und klassischen Stil aufgebaut. Ob für Geschichtsstunden oder für die Mathematik, dieses Asset-Pack ist perfekt für das Dastellen eines alten Arbeitszimmers.</p><p>&nbsp;</p><p>Quelle: <a href=https://assetstore.unity.com/>https://assetstore.unity.com/</a></p>", 90, false, false, "https://assetstorev1-prd-cdn.unity3d.com/package-screenshot/2ece25b7-2046-4dda-840a-b8aa0b6a2dd9.webp", "https://assetstore.unity.com/packages/3d/environments/alchemist-house-112442", "Unity", "Klassisches 3D-Arbeitszimmer ", false, false, false, false, false, false },
                    { 93, false, "<p>Dieses Asset-Pack enthält hochwertige Boden-Texturen für den Außenbereich, darunter Erde, Gras, Kies, und Sand. Ideal für realistische Landschaftsgestaltung in Ihren Projekten.</p><p>&nbsp;</p><p>Quelle: <a href=https://assetstore.unity.com/>https://assetstore.unity.com/</a></p>", 93, false, false, "https://assetstorev1-prd-cdn.unity3d.com/key-image/ee3784ba-32ac-4d90-b761-f490b5fda910.webp", "https://assetstore.unity.com/packages/2d/textures-materials/floors/outdoor-ground-textures-12555", "Unity", "Boden-Texturen (außen)", false, false, false, false, false, false },
                    { 96, false, "<p>Dieses Asset-Pack bietet hochwertige Boden-Texturen für Innenräume, darunter verschiedene Parkettböden aus unterschiedlichen Holzarten. Perfekt geeignet für realistische Raumgestaltung in Spielen, Architekturvisualisierungen und Simulationen.</p><p>&nbsp;</p><p>Quelle: <a href=https://assetstore.unity.com/>https://assetstore.unity.com/</a></p>", 96, false, false, "https://assetstorev1-prd-cdn.unity3d.com/key-image/95454c62-09f4-421e-b63e-db7577369d98.webp", "https://assetstore.unity.com/packages/2d/textures-materials/wood/yughues-free-wooden-floor-materials-13213", "Unity", "Boden-Texturen (innen)", false, false, false, false, false, false },
                    { 100, false, "<p>Dieses Asset-Pack enthält eine vielfältige Auswahl an Wand-Texturen, wodurch Sie Ihren Projekten realistische Wände verleihen können.</p><p>&nbsp;</p><p>Quelle: <a href=https://assetstore.unity.com/>https://assetstore.unity.com/</a></p>", 100, false, false, "https://assetstorev1-prd-cdn.unity3d.com/key-image/ae547247-7b3d-4eeb-98d0-58c534b94785.webp", "https://assetstore.unity.com/packages/2d/textures-materials/brick/p3d-outdoor-wall-tile-texture-pack-lr-247739", "Unity", "Wand-Texturen", false, false, false, false, false, false },
                    { 104, false, "<p>Dieses kostenlose Asset-Pack enthält eine Büroeinrichtung und -zubehör. Enthalten sind unter Anderem:&nbsp;</p><ul><li>Arbeitstische&nbsp;</li><li>Regale und Bücherregale (inklusive Bücher zum einsetzen)&nbsp;</li><li>verschiedene gemütliche Sitzgelegenheiten</li><li>Drucker</li><li>Schreibtischlampe</li><li>Zimmerpflanzen</li><li>Wasserspender</li></ul><p>Quelle: <a href=https://assetstore.unity.com>https://assetstore.unity.com</a></p>", 104, false, false, "https://assetstorev1-prd-cdn.unity3d.com/key-image/c3f26526-8631-4d82-a2fa-9cb916ea00f7.webp", "https://assetstore.unity.com/packages/3d/props/interior/office-pack-free-258600", "Unity ", "3D-Büroeinrichtung und Zubehör", false, false, false, false, false, false },
                    { 105, false, "<p>Dieses Asset-Pack enthält detaillierte 3D-Modelle rund um das Thema Schule, darunter Klassenzimmer, Schulmöbel, Tafeln, Bücher und vieles mehr. Gestalten Sie Ihre Traumschule wie Sie es möchten.</p><p>&nbsp;</p><p>Quelle: <a href=https://assetstore.unity.com/>https://assetstore.unity.com/</a></p>", 105, false, false, "https://assetstorev1-prd-cdn.unity3d.com/key-image/a2d3d3be-1539-4b59-944e-57d78d1ed352.webp", "https://assetstore.unity.com/packages/3d/environments/school-assets-146253", "Unity", "3D-Schulmodelle", false, false, false, false, false, false },
                    { 106, false, "<p>Dieses kostenlose Asset- Pack enthält ein Set aus Sportzubehör für folgende Sportarten:&nbsp;</p><ul><li>Fußball</li><li>Basketball</li><li>Tennis</li><li>Golf</li><li>Volleyball</li></ul><p>Quelle: <a href=https://assetstore.unity.com>https://assetstore.unity.com</a></p>", 106, false, false, "https://assetstorev1-prd-cdn.unity3d.com/key-image/a22ebac6-e282-4b96-915e-5acbac35ff32.webp", "https://assetstore.unity.com/packages/3d/characters/free-sports-kit-239377", "Unity ", "3D-Sportset", false, false, false, false, false, false },
                    { 107, false, "<p>Dieses Asset- Pack enthält ein Set aus verschiedenen Blumen- perfekt für ein Blumenbeet oder eine Landschaft!</p><p>Quelle: <a href=https://assetstore.unity.com>https://assetstore.unity.com</a></p>", 107, false, false, "https://assetstorev1-prd-cdn.unity3d.com/key-image/ce36211b-6732-4565-b5df-ec0ca2706c53.webp", "https://assetstore.unity.com/packages/3d/vegetation/plants/lowpoly-flowers-47083", "Unity ", "3D-Blumen ", false, false, false, false, false, false },
                    { 108, false, "<p>Dieses vielseitige Asset-Pack umfasst 10 detaillierte 3D-Szenen mit unterschiedlichen Themen und Umgebungen - darunter Stadtlandschaften, Dörfer, Häfen, Straßen mit Fahrzeugen, Schiffe und mehr. Die Szenen können auch beliebig umgestaltet werden. Ihrer Kreativität sind keine Grenzen gesetzt.</p><p>&nbsp;</p><p>Quelle: <a href=https://assetstore.unity.com/>https://assetstore.unity.com/</a></p>", 108, false, false, "https://assetstorev1-prd-cdn.unity3d.com/key-image/6418ff06-409e-4702-953d-10ef07657523.webp", "https://assetstore.unity.com/packages/3d/environments/landscapes/low-poly-atmospheric-locations-pack-278928", "Unity", "3D-Szenen", false, false, false, false, false, false },
                    { 109, false, "<p>Dieses Asset-Pack stellt einen mittelalterlichen Marktplatz dar und enthält detailreiche 3D-Modelle von Marktständen, Fisch, Obst, Gemüse, Körben und Tischen. Ideal für historische Spiele, Simulationen und Visualisierungen mit authentischem Mittelalter-Flair.</p><p>&nbsp;</p><p>Quelle: <a href=https://assetstore.unity.com/>https://assetstore.unity.com/</a></p>", 109, false, false, "https://assetstorev1-prd-cdn.unity3d.com/key-image/964fc471-ec95-4388-a3d9-9917540a40dd.webp", "https://assetstore.unity.com/packages/3d/environments/low-poly-medieval-market-262473", "Unity", "3D-Marktplatz", false, false, false, false, false, false },
                    { 110, false, "<p>Dieses kostenlose Asset-Pack enthält eine Kantineneinrichtung inklusive Geschirr, Besteck und einer Vielzahl an unterschiedlichen Speisen.&nbsp;</p><p>Quelle: <a href=https://assetstore.unity.com>https://assetstore.unity.com</a></p>", 110, false, false, "https://assetstorev1-prd-cdn.unity3d.com/key-image/ec0f600d-b21e-4f72-9f7b-f3c3b30b52a1.webp", "https://assetstore.unity.com/packages/3d/props/food/pandazole-kitchen-food-low-poly-pack-204525", "Unity ", "3D-Kantine (inklusive Essen) ", false, false, false, false, false, false },
                    { 111, false, "<p>Dieses Low-Poly-Asset-Pack zeigt ein stilisiertes Büro mit einer Vielzahl an Geräten und Möbeln, darunter Schreibtische, Stühle, Computer, Drucker, Telefone und mehr. Perfekt geeignet für mobile Spiele, Simulationen oder stilisierte Visualisierungen im Büro-Setting.</p><p>&nbsp;</p><p>Quelle: <a href=https://assetstore.unity.com/>https://assetstore.unity.com/</a></p>", 111, false, false, "https://assetstorev1-prd-cdn.unity3d.com/key-image/3c01bc3b-a0f5-4872-9bc8-e8bc7ff904e1.webp", "https://assetstore.unity.com/packages/3d/characters/low-poly-office-pack-characters-props-119386", "Unity", "3D-Büro", false, false, false, false, false, false },
                    { 112, false, "<p>Dieses Asset-Pack enthält eine Auswahl an 3D-Stühlen in verschiedenen Farben und Designs. Ideal für Innenraumgestaltungen, Visualisierungen und stylisierte Szenen mit variabler Möblierung. Zudem ist es perfekt für die 6-Denkhüte-Methode nach de Bono.</p><p>&nbsp;</p><p>Quelle: <a href=https://assetstore.unity.com/>https://assetstore.unity.com/</a></p>", 112, false, false, "https://assetstorev1-prd-cdn.unity3d.com/package-screenshot/2f34e519-10ad-4fce-8989-b812e2f0a272.webp", "https://assetstore.unity.com/packages/3d/environments/low-poly-office-props-lite-131438", "Unity", "3D-Stuhlmodelle", false, false, false, false, false, false },
                    { 113, false, "<p>Dieses Asset-Pack enthält ein detailreiches 3D-Segelschiff-Modell mit begehbaren Kabinen und Unterdeck. Perfekt geeignet für Spiele, Simulationen, Animationen oder Visualisierungen mit maritimen Szenen.</p><p>&nbsp;</p><p>Quelle: <a href=https://assetstore.unity.com/>https://assetstore.unity.com/</a></p>", 113, false, false, "https://assetstorev1-prd-cdn.unity3d.com/key-image/0d476720-573d-4289-890e-d91daf0e089e.webp", "https://assetstore.unity.com/packages/3d/vehicles/sea/stylized-pirate-ship-200192", "Unity", "3D-Segelschiff", false, false, false, false, false, false },
                    { 114, false, "<p>Das GitHub-Repository <a href=https://github.com/agmmnn/awesome-blender?tab=readme-ov-file#table><strong>agmmnn/awesome-blender</strong></a> bietet eine umfangreiche Sammlung sorgfältig kuratierter Ressourcen für Blender-Nutzer. Diese Sammlung richtet sich an 3D-Künstler, Entwickler, Hobbyisten und Forscher und legt den Fokus auf kostenlose und quelloffene Inhalte.</p>", 114, false, false, "/images/makerspace/7c473572-2aa1-4d41-be26-cd11fb45a7b3.png", "https://github.com/agmmnn/awesome-blender", "Blender", "Awesome-Blender ", false, false, false, false, false, false },
                    { 116, false, "<p>Bring Leben in deine Waldlandschaften mit diesem Nadelbaum-Asset-pack. Die vier unterschiedlichen Varianten ermöglichen es dir, einen natürlich und lebendig wirkenden Nadelwald zu gestalten.</p><p>Quelle: <a href=https://assetstore.unity.com/>https://assetstore.unity.com/</a></p>", 116, false, false, "https://assetstorev1-prd-cdn.unity3d.com/key-image/b7488e8a-654e-4b06-937f-09c471292891.webp", "https://assetstore.unity.com/packages/3d/vegetation/trees/conifers-botd-142076", "Unity", "3D-Nadelbäume", false, false, false, false, false, false },
                    { 117, false, "<p>Erstelle lebendige, detailreiche Stadtlandschaften mit diesem vielseitigen Asset-Pack. Mit einer breiten Auswahl an Gebäuden, Fahrzeugen, Natur-Assets und Stadtelementen bietet es dir alles, was du für die Gestaltung städtischer Szenen brauchst- ganz gleich, ob es sich um einen ruhigen Vorort oder das lebendige Stadtzentrum handelt.&nbsp;</p><p>Quelle: <a href=https://assetstore.unity.com/>https://assetstore.unity.com/</a></p>", 117, false, false, "https://assetstorev1-prd-cdn.unity3d.com/key-image/c3b61fc1-009c-43e3-a192-f09a7d30753e.webp", "https://assetstore.unity.com/packages/3d/environments/simplepoly-city-low-poly-assets-58899", "Unity ", "3D-Stadtlandschaft", false, false, false, false, false, false },
                    { 118, false, "<p>Hauche deiner Spielumgebung Leben ein mit diesem Asset-Pack, das sieben animierte Tiere enthält. Ob Dschungel, Bauernhof oder Wohnzimmer- diese animierten Tiere fügen sich nahtlos in verschiedenste Low-Poly Spielwelten ein.&nbsp;</p><p>Quelle: <a href=https://assetstore.unity.com/>https://assetstore.unity.com/</a></p>", 118, false, false, "https://assetstorev1-prd-cdn.unity3d.com/key-image/7247dd64-b9e5-43c6-96f5-c34b6c0a49e5.webp", "https://assetstore.unity.com/packages/3d/characters/animals/animals-free-animated-low-poly-3d-models-260727", "Unity ", "Animierte Tiere (Low-Poly)", false, false, false, false, false, false },
                    { 119, false, "<p>Erschaffe eine atmosphärische Wikingerwelt mit diesem Umgebungspaket. Ob am Fjord, Seeufer oder in rauer Wildnis- mit detailreichen Hütten, Zäunen, Fässern, Fackeln, Vegetation und vielem mehr lässt sich ein authentisches und lebendiges Dorf voller Geschichte gestalten.&nbsp;</p><p>Quelle: <a href=https://assetstore.unity.com/>https://assetstore.unity.com/</a></p>", 119, false, false, "https://assetstorev1-prd-cdn.unity3d.com/key-image/a6587a98-62df-4b0f-a441-ce33836990e8.webp", "https://assetstore.unity.com/packages/essentials/tutorial-projects/viking-village-urp-29140", "Unity", "Wikingerdorf", false, false, false, false, false, false },
                    { 120, false, "<p>Baue eine moderne Stadt mit diesem umfangreichen Asset-Pack, das eine große Auswahl an Gebäuden, Straßen und städtischer Vegetation bietet. Von Polizei, Feuerwehr und Krankenhaus bis hin zu Wohnhäusern, Läden und einer Bank- dieses Asset-Pack bietet 45 verschiedene Gebäude. Ergänzt durch 189 Props wie Straßenschilder, Mülltonnen, Parkbänke, Zäune und mehr, kannst du damit deine Stadt nach deinen individuellen Vorstellungen modellieren.</p><p>Quelle: <a href=https://assetstore.unity.com/>https://assetstore.unity.com/</a></p>", 120, false, false, "https://assetstorev1-prd-cdn.unity3d.com/package-screenshot/bc411d15-8ece-4ecb-8cef-edaf336f5e84.webp", "https://assetstore.unity.com/packages/3d/environments/urban/city-package-107224", "Unity ", "Moderne Stadtlandschaft", false, false, false, false, false, false },
                    { 121, false, "<p>Dieses Asset-Pack enthält 12 detailreiche und realistische Gras- und Blumen-Texturen mit denen farbenfrohe Blumenwiesen gestaltet werden können.&nbsp;</p><p>Quelle: <a href=https://assetstore.unity.com/>https://assetstore.unity.com/</a></p>", 121, false, false, "https://assetstorev1-prd-cdn.unity3d.com/package-screenshot/7df2a515-3194-4fc8-8df1-db459fe9e01b.webp", "https://assetstore.unity.com/packages/2d/textures-materials/nature/grass-flowers-pack-free-138810", "Unity ", "3D-Blumenwiese", false, false, false, false, false, false },
                    { 122, false, "<p>Tauche ein in eine postapokalyptische Atmosphäre mit diesem Asset-Pack einer gefluteten Stadt. Enthalten sind Industriegebäude, Wohnhäuser, Kirchen und mehr- alle vom Wasser beschädigt und verlassen. Ideal als Vorlage für inspirierende Storys, Survival-Szenarien oder stimmungsvolle Umgebungen.&nbsp;</p><p>Quelle: <a href=https://assetstore.unity.com/>https://assetstore.unity.com/</a></p>", 122, false, false, "https://assetstorev1-prd-cdn.unity3d.com/key-image/10ff3322-8a41-4636-bade-bf7fff81406d.webp", "https://assetstore.unity.com/packages/3d/environments/flooded-grounds-48529", "Unity ", "Geflutete Stadt", false, false, false, false, false, false },
                    { 123, false, "<p>Erstelle Low-Poly Naturlandschaften mit diesem modularen Umgebungspaket. Es bietet eine Auswahl an Bergen, Klippen, Wasserfällen, Häusern, Straßen und Fahrzeugen.</p><p>Quelle: <a href=https://assetstore.unity.com/>https://assetstore.unity.com/</a></p>", 123, false, false, "https://assetstorev1-prd-cdn.unity3d.com/key-image/e70bead0-b0a8-4684-90ad-4b8a2aeb852a.webp", "https://assetstore.unity.com/packages/3d/environments/low-poly-environment-315184", "Unity ", "Naturumgebung (Low-Poly)", false, false, false, false, false, false },
                    { 124, false, "<p>Dieses Asset-Pack enthält 15 Holztür-Modelle, die sich vollständig öffnen und animieren lassen. Mit 105 farblichen Prefab-Varianten bietet es maximale Vielfalt für stilistisch unterschiedliche Szenen.</p><p>Quelle: <a href=https://assetstore.unity.com/>https://assetstore.unity.com/</a></p>", 124, false, false, "https://assetstorev1-prd-cdn.unity3d.com/key-image/d0430e4b-b630-4e4d-942e-658a635097ae.webp", "https://assetstore.unity.com/packages/3d/props/interior/free-wood-door-pack-280509", "Unity ", "3D-Holztüren ", false, false, false, false, false, false },
                    { 125, false, "<p>Dieses Asset-Pack enthält 51 realistische abwechslungsreiche Baum-Modelle zur Gestaltung von Naturumgebungen. Ideal zur Gestaltung von Wäldern, Parks oder offenen Landschaften.&nbsp;</p><p>Quelle: <a href=https://assetstore.unity.com/>https://assetstore.unity.com/</a></p>", 125, false, false, "https://assetstorev1-prd-cdn.unity3d.com/key-image/c363896c-5ce6-486e-bd7c-a48b0c0be35c.webp", "https://assetstore.unity.com/packages/3d/vegetation/trees/european-forests-realistic-trees-229716", "Unity ", "Waldpaket- 51 realistische Baummodelle", false, false, false, false, false, false },
                    { 126, false, "<p>Microsplat ist ein Terrain-Shading-System für Unity, das realistische und hochperformative Bodendarstellungen ermöglicht. Mit erweiterten Funktionen wie dynamischer Nässe, Schnee, Triplanar-Mapping und optimiertem Texturhandling hebt es Terrain-Gestaltung auf ein neues Niveau.&nbsp;</p><p>Quelle: <a href=https://assetstore.unity.com/>https://assetstore.unity.com/</a></p>", 126, false, false, "https://assetstorev1-prd-cdn.unity3d.com/key-image/1ee333b4-4d58-4581-9693-6be5ea21e9e8.webp", "https://assetstore.unity.com/packages/tools/terrain/microsplat-96478", "Unity ", "MicroSplat", false, false, false, false, false, false },
                    { 127, false, "<p>Dieses Asset-Pack bietet eine atmosphärische Café-Umgebung mit Möbeln, Theken, Geräten und sorgfältig gestalteten Details.&nbsp;</p><p>Quelle: <a href=https://assetstore.unity.com/>https://assetstore.unity.com/</a></p>", 127, false, false, "https://assetstorev1-prd-cdn.unity3d.com/key-image/bb868e29-1d38-4a11-b03e-c58a79bcebda.webp", "https://assetstore.unity.com/packages/3d/environments/coffee-shop-environment-217600", "Unity ", "Café ", false, false, false, false, false, false },
                    { 128, false, "<p>Dieses Asset-Pack enthält eine breite Auswahl an Möbeln. Enthalten sind unter Anderem:&nbsp;</p><ul><li>Stühle</li><li>Sofa</li><li>Bett</li><li>Kommoden</li><li>Schränke&nbsp;</li><li>Tische</li><li>Klavier</li><li>Hantelbank</li></ul><p>Quelle: <a href=https://assetstore.unity.com/>https://assetstore.unity.com/</a></p>", 128, false, false, "https://assetstorev1-prd-cdn.unity3d.com/key-image/f5461b83-77fa-4c4f-9100-fa9d6ad57bff.webp", "https://assetstore.unity.com/packages/3d/props/furniture/furniture-free-low-poly-3d-models-pack-260522", "Unity ", "Low-Poly Einrichtung", false, false, false, false, false, false },
                    { 129, false, "<p>Dieses Asset-Pack enthält realistisch gestaltete Gebäudeteile, Rohre und Wracks, die eine authentische verlassene Fabrik-Atmosphäre schaffen.&nbsp;</p><p>Quelle: <a href=https://assetstore.unity.com/>https://assetstore.unity.com/</a></p>", 129, false, false, "https://assetstorev1-prd-cdn.unity3d.com/key-image/2b9bd254-ca97-4f8d-8199-1b0aabb3b98e.webp", "https://assetstore.unity.com/packages/3d/props/industrial/abandoned-factory-lite-62597", "Unity ", "Verlassenes Fabrikgelände ", false, false, false, false, false, false },
                    { 130, false, "<p>Dieses riesige Asset-Pack enthält mehr als 1650 Low-Poly-Modelle. Von Pflanzen, Möbeln und Gebäuden bis hin zu Straßenschildern, Böden, Flüssen, Wegen und einer großen Auswahl an Speisen- ist alles enthalten, um eine Low-Poly Umgebung nach eigenen Vorstellungen zu schaffen.&nbsp;</p><p>Quelle: <a href=https://assetstore.unity.com/>https://assetstore.unity.com/</a></p>", 130, false, false, "https://assetstorev1-prd-cdn.unity3d.com/key-image/dc7e97fd-3ebb-4ff8-bf23-2f2293bca7c4.webp", "https://assetstore.unity.com/packages/3d/props/pandazole-lowpoly-asset-bundle-226938", "Unity ", "Umfangreiches Low-Poly Umgebungspaket", false, false, false, false, false, false },
                    { 131, false, "<p>Dieses Asset-Pack enthält eine vollständig möblierte Hütte inklusive Dekoration und strukturellem Aufbau. Perfekt geeignet für Umgebungen mit gemütlicher Atmosphäre.&nbsp;</p><p>Quelle: <a href=https://assetstore.unity.com/>https://assetstore.unity.com/</a></p>", 131, false, false, "https://assetstorev1-prd-cdn.unity3d.com/package-screenshot/446ae928-a3b6-4886-ad65-7b8f3e967b47.webp", "https://assetstore.unity.com/packages/3d/environments/urban/furnished-cabin-71426", "Unity ", "Möblierte Hütte", false, false, false, false, false, false },
                    { 132, false, "<p>Diese Foliage Engine ermöglicht die performative Darstellung (inklusive Windanimation) von Gräsern, Büschen, Bäumen und anderen Pflanzen.&nbsp;</p><p>Quelle: <a href=https://assetstore.unity.com/>https://assetstore.unity.com/</a></p>", 132, false, false, "https://assetstorev1-prd-cdn.unity3d.com/key-image/2c302d82-8a01-4179-b98d-05d69f885feb.webp", "https://assetstore.unity.com/packages/vfx/shaders/the-toby-foliage-engine-light-282901", "Unity ", "Foliage Engine Light- effiziente Vegetationslösung für Unity ", false, false, false, false, false, false },
                    { 133, false, "<p>Dieses Asset-Pack enthält Wände, Böden, Türen, Requisiten und mehr zur Gestaltung von futuristischen Umgebungen. Es eignet sich optimal zur Erstellung von Raumschiff-Umgebungen.&nbsp;</p><p>Quelle: <a href=https://assetstore.unity.com/>https://assetstore.unity.com/</a></p>", 133, false, false, "https://assetstorev1-prd-cdn.unity3d.com/key-image/6bad829f-06a4-4bbd-aa4b-591ef2c3f6fc.webp", "https://assetstore.unity.com/packages/3d/environments/3d-scifi-kit-starter-kit-92152", "Unity ", "Modulares Science-Fiction-Pack", false, false, false, false, false, false },
                    { 134, false, "<p>Dieses Asset-Pack enthält Bäume, Beete, Tomaten- und Salat-Pflanzen und Körbe, die mit der Ernte gefüllt sind. Die perfekte Wahl für die Erstellung von Low-Poly Farmumgebungen oder ländlichen Szenen.</p><p>Quelle: <a href=https://assetstore.unity.com/>https://assetstore.unity.com/</a></p>", 134, false, false, "https://assetstorev1-prd-cdn.unity3d.com/key-image/0f20fb06-37b0-4e5e-80d4-0d99c189edc7.webp", "https://assetstore.unity.com/packages/3d/environments/industrial/low-poly-farm-pack-lite-188100", "Unity ", "Low-Poly Farm", false, false, false, false, false, false },
                    { 135, false, "<p>Dieses Asset-Pack bietet alles, was du für die Erstellung eines typischen Fast-Food-Restaurants brauchst. Enthalten sind unter Anderem ein typisches Straßenschild, Sitzgelegenheiten, Getränkeautomaten, Kassen und digitale Bestellterminale.</p><p>Quelle: <a href=https://assetstore.unity.com/>https://assetstore.unity.com/</a></p>", 135, false, false, "https://assetstorev1-prd-cdn.unity3d.com/key-image/14253f1a-ceea-42ca-99f2-5806003a400c.webp", "https://assetstore.unity.com/packages/3d/environments/fast-food-restaurant-kit-239419", "Unity ", "Fast-Food Restaurant", false, false, false, false, false, false },
                    { 136, false, "<p>Dieses Asset-Pack bietet eine Auswahl an Bäumen, Pilzen, Gräsern und Blumen zur Ergänzung bestehender Szenen oder zum Erstellen eigener Waldumgebungen.&nbsp;</p><p>Quelle: <a href=https://assetstore.unity.com/>https://assetstore.unity.com/</a></p>", 136, false, false, "https://assetstorev1-prd-cdn.unity3d.com/key-image/853fc874-adeb-4294-9e1d-7b8b783b63e0.webp", "https://assetstore.unity.com/packages/3d/environments/3d-nature-assetspack-215646", "Unity ", "3D-Waldvegetation", false, false, false, false, false, false },
                    { 137, false, "<p>Mit drei verschiedenen Küchenvarianten- blau, rosa und grün- sowie 40 verschiedenen Modellen bietet das Asset-Pack vielfältige Möglichkeiten zur Gestaltung von Küchenumgebungen.&nbsp;</p><p>Quelle: <a href=https://assetstore.unity.com/>https://assetstore.unity.com/</a></p>", 137, false, false, "https://assetstorev1-prd-cdn.unity3d.com/key-image/faecb0ac-8265-4961-bcf9-ccb643ad302f.webp", "https://assetstore.unity.com/packages/3d/props/interior/free-kitchen-cabinets-and-equipment-245554", "Unity ", "3D-Küchenset", false, false, false, false, false, false },
                    { 138, false, "<p>Mit einer großen Auswahl an Spielgeräten wie Klettergerüsten, Rutschen, Schaukeln und mehr bietet dieses Asset-Pack alles, was für einen spannend gestalteten Spielplatz nötig ist. Optimal geeignet zur Gestaltung von Parks, Schulhöfen und familienfreundlichen Umgebungen.&nbsp;</p><p>Quelle: <a href=https://assetstore.unity.com/>https://assetstore.unity.com/</a></p>", 138, false, false, "https://assetstorev1-prd-cdn.unity3d.com/key-image/49d00609-3ed1-4e7c-87c9-8cde8ee26f51.webp", "https://assetstore.unity.com/packages/3d/environments/playground-low-poly-191533", "Unity ", "3D-Spielplatz ", false, false, false, false, false, false },
                    { 139, false, "<p>Dieses Asset-Pack enthält eine vielfältige Sammlung von 3D-Lebensmittelmodellen&nbsp;- von frischem Obst und Gemüse bis hin zu Pizza und Fleisch. Ideal für den Einsatz in Spielen, Simulationen, Visualisierungen oder virtuellen Marktszenen.</p><p>&nbsp;</p><p>Quelle: <a href=https://assetstore.unity.com/>https://assetstore.unity.com/</a></p>", 139, false, false, "https://assetstorev1-prd-cdn.unity3d.com/key-image/12ef1741-7aee-488c-8f06-1e0e6c3bc67d.webp", "https://assetstore.unity.com/packages/3d/props/food/food-pack-free-demo-225294", "Unity", "3D-Lebensmittel", false, false, false, false, false, false },
                    { 140, false, "<p>Dieses Asset-Pack bietet eine Auswahl an hochwertigen Boden-Texturen für Innen- und Außenbereiche - von edlem Parkett und Fliesen bis hin zu Naturstein, Asphalt und Erde. Perfekt geeignet für realistische Visualisierungen in Ihrem Projekt.</p><p>&nbsp;</p><p>Quelle: <a href=https://assetstore.unity.com/>https://assetstore.unity.com/</a></p>", 140, false, false, "https://assetstorev1-prd-cdn.unity3d.com/key-image/7e018605-383e-416e-ba92-29599a2cc8f4.webp", "https://assetstore.unity.com/packages/2d/textures-materials/25-free-realistic-textures-nature-city-home-construction-more-240323", "Unity", "Boden-Texturen (universal)", false, false, false, false, false, false },
                    { 141, false, "<p>Dieses Asset-Pack enthält realistische 3D-Modelle einer vollständigen Inneneinrichtung - von Möbeln und Dekoration bis hin zu alltäglichen Haushaltsgegenständen. Ideal für Architekturvisualisierungen, Interior-Design-Konzepte und virtuelle Umgebungen</p><p>&nbsp;</p><p>Quelle: <a href=https://assetstore.unity.com/>https://assetstore.unity.com/</a></p>", 141, false, false, "https://assetstorev1-prd-cdn.unity3d.com/key-image/6d55a8a0-16a8-4bf4-a5df-90a1846b41c1.webp", "https://assetstore.unity.com/packages/3d/environments/urban/devden-archviz-vol-1-scotland-urp-204933", "Unity", "Realistische Inneneinrichtung", false, false, false, false, false, false },
                    { 142, false, "<p>Verleihen Sie Ihren Projekten mehr Leben mit SeedMesh Shader! Es enthält einen anpassbaren Shader, der realistische Bewegungen für Vegetation und Pflanzen erzeugt - vom sanften Rascheln der Blätter bis hin zum rhythmischen Wiegen ganzer Sträucher im Wind. Ideal für stylisierte oder realistische Umgebungen in Unity.</p><p>&nbsp;</p><p>Quelle: <a href=https://assetstore.unity.com/>https://assetstore.unity.com/</a></p>", 142, false, false, "https://assetstorev1-prd-cdn.unity3d.com/key-image/8467f739-5d81-4837-8d3a-9a0a8e042c7e.webp", "https://assetstore.unity.com/packages/vfx/shaders/seedmesh-vegetation-shaders-232690", "Unity", "SeedMesh Shader für Pflanzen", false, false, false, false, false, false },
                    { 143, false, "<p>Bringen Sie Vielfalt und Realismus in Ihre Umgebungen mit diesem hochwertigen Assetpack! Enthalten sind 17 verschiedene 3D-Modelle von Birken, die stark für eine herausragende Performance optimiert sind.</p><p>&nbsp;</p><p>Quelle: <a href=https://assetstore.unity.com/>https://assetstore.unity.com/</a></p>", 143, false, false, "https://assetstorev1-prd-cdn.unity3d.com/key-image/2069e1ba-e668-4b54-b19d-c53cfab558fe.webp", "https://assetstore.unity.com/packages/3d/vegetation/trees/urp-white-birch-tree-mobile-281448", "Unity", "3D-Birken", false, false, false, false, false, false },
                    { 144, false, "<p>Tauchen Sie ein in die Welt vergangener Zeiten mit diesem detailreichen 3D-Assetpack eines mittelalterlichen Lernzimmers. Es enthält liebevoll gestaltete Möbel, Bücher, Wanddekorationen und weiteres Zubehör, um eine authentische und stimmungsvolle Lernumgebung zu erschaffen.</p><p>&nbsp;</p><p>Quelle: <a href=https://assetstore.unity.com/>https://assetstore.unity.com/</a></p>", 144, false, false, "https://assetstorev1-prd-cdn.unity3d.com/key-image/ab56a4ba-ebc6-48ea-87ab-8eb8a19ea183.webp", "https://assetstore.unity.com/packages/3d/environments/free-medieval-room-131004", "Unity", "Mittelalterliches Lernzimmer", false, false, false, false, false, false },
                    { 145, false, "<p>Erweitern Sie Ihre Projekte mit diesem vielseitigen Assetpack, welches Ihnen ermöglicht, Ihre Szenen mit prozedural erzeugten Objekten wie Bäumen, Steinen und weiteren Naturdetails zu bereichern. Durch die automatische Variation entstehen natürliche und abwechslungsreiche Umgebungen, die Ihre Welten lebendiger und realistischer wirken lassen.</p><p>&nbsp;</p><p>Quelle: <a href=https://assetstore.unity.com/>https://assetstore.unity.com/</a></p>", 145, false, false, "https://assetstorev1-prd-cdn.unity3d.com/key-image/80ed08cd-d9dc-4dec-8600-317716482eff.webp", "https://assetstore.unity.com/packages/tools/terrain/mega-world-free-207301", "Unity", "Procedual World Generation", false, false, false, false, false, false }
                });

            migrationBuilder.UpdateData(
                table: "PortalVideo",
                keyColumn: "Id",
                keyValue: 1,
                column: "UploadDate",
                value: new DateTime(2025, 10, 28, 13, 32, 27, 880, DateTimeKind.Local).AddTicks(8435));

            migrationBuilder.InsertData(
                table: "PromptAiSettings",
                columns: new[] { "Id", "FilterSystemPreamble", "KiAssistantSystemPrompt", "SmartSelectionSystemPreamble", "SmartSelectionUserPrompt", "SystemPreamble", "UpdatedAt" },
                values: new object[] { 1, "", "", "", "", "Du bist eine expert*innengeführte Assistenz für Instructional Design, Hochschuldidaktik und wissenschaftliches Arbeiten.\r\nSchreibe in präzisem, formalem Deutsch für Forschung & Lehre. Nutze wissenschaftliche Terminologie.\r\nHalte dich strikt an das JSON-Schema (Structured Outputs). Antworte ausschließlich mit JSON.\r\nJedes Filter-Item muss eine überprüfbare Leistung erzeugen (Artefakt/Metrik/Kriterium, Quellenstandard).", new DateTime(2025, 10, 28, 12, 32, 27, 880, DateTimeKind.Utc).AddTicks(7862) });

            migrationBuilder.InsertData(
                table: "PromptTypeGuidances",
                columns: new[] { "Id", "GuidanceText", "Type", "UpdatedAt" },
                values: new object[,]
                {
                    { 1, "Fokus: Schreiben, Quellenarbeit, Rollen, formative Checks. Platzhalter: {Niveau}, {Zielgruppe}, {Textsorte}.", 0, new DateTime(2025, 10, 28, 12, 32, 27, 880, DateTimeKind.Utc).AddTicks(7936) },
                    { 2, "Fokus: Bild-Generierung (Motiv, Stil, Komposition, Licht). Platzhalter: {Motiv}, {Stil}, {Farbpalette}, {Kamera}.", 1, new DateTime(2025, 10, 28, 12, 32, 27, 880, DateTimeKind.Utc).AddTicks(7937) },
                    { 3, "Fokus: Video (Szenenplan, Shots, Voice-over, Untertitel). Platzhalter: {Dauer}, {Szenen}, {VoiceOver}, {Untertitel}.", 2, new DateTime(2025, 10, 28, 12, 32, 27, 880, DateTimeKind.Utc).AddTicks(7938) },
                    { 4, "Fokus: Audio/Musik (Genre, BPM, Tonart, Struktur). Platzhalter: {Genre}, {BPM}, {Instrumente}, {Stimmung}.", 3, new DateTime(2025, 10, 28, 12, 32, 27, 880, DateTimeKind.Utc).AddTicks(7939) },
                    { 5, "Fokus: Bloom, Forschungsfragen, Methodik, Zitieren, Rubrics. Platzhalter: {Fach}, {Niveau}, {Lernziel}, {Bewertungskriterien}.", 4, new DateTime(2025, 10, 28, 12, 32, 27, 880, DateTimeKind.Utc).AddTicks(7939) },
                    { 6, "Fokus: Meta-Prompting, Evaluation, Iteration, Checklisten. Platzhalter: {Kriterien}, {Revision}, {Feedbackschleifen}.", 5, new DateTime(2025, 10, 28, 12, 32, 27, 880, DateTimeKind.Utc).AddTicks(7940) },
                    { 7, "Fokus: Benutzerdefinierte/Eigene Filter. Platziere begründete Freiheitsgrade in den Platzhaltern.", 6, new DateTime(2025, 10, 28, 12, 32, 27, 880, DateTimeKind.Utc).AddTicks(7941) }
                });

            migrationBuilder.UpdateData(
                table: "Skills",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2025, 10, 28, 13, 32, 27, 880, DateTimeKind.Local).AddTicks(8159));

            migrationBuilder.CreateIndex(
                name: "IX_AssistantEmbeddings_AssistantId",
                table: "AssistantEmbeddings",
                column: "AssistantId");

            migrationBuilder.CreateIndex(
                name: "IX_SemanticIndexEntries_EntityType_EntityId",
                table: "SemanticIndexEntries",
                columns: new[] { "EntityType", "EntityId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AssistantEmbeddings");

            migrationBuilder.DropTable(
                name: "SemanticIndexEntries");

            migrationBuilder.DropTable(
                name: "Assistants");

            migrationBuilder.DeleteData(
                table: "MakerSpaceProjects",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "MakerSpaceProjects",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "MakerSpaceProjects",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "MakerSpaceProjects",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "MakerSpaceProjects",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "MakerSpaceProjects",
                keyColumn: "Id",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "MakerSpaceProjects",
                keyColumn: "Id",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "MakerSpaceProjects",
                keyColumn: "Id",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "MakerSpaceProjects",
                keyColumn: "Id",
                keyValue: 10);

            migrationBuilder.DeleteData(
                table: "MakerSpaceProjects",
                keyColumn: "Id",
                keyValue: 11);

            migrationBuilder.DeleteData(
                table: "MakerSpaceProjects",
                keyColumn: "Id",
                keyValue: 13);

            migrationBuilder.DeleteData(
                table: "MakerSpaceProjects",
                keyColumn: "Id",
                keyValue: 14);

            migrationBuilder.DeleteData(
                table: "MakerSpaceProjects",
                keyColumn: "Id",
                keyValue: 18);

            migrationBuilder.DeleteData(
                table: "MakerSpaceProjects",
                keyColumn: "Id",
                keyValue: 19);

            migrationBuilder.DeleteData(
                table: "MakerSpaceProjects",
                keyColumn: "Id",
                keyValue: 20);

            migrationBuilder.DeleteData(
                table: "MakerSpaceProjects",
                keyColumn: "Id",
                keyValue: 21);

            migrationBuilder.DeleteData(
                table: "MakerSpaceProjects",
                keyColumn: "Id",
                keyValue: 23);

            migrationBuilder.DeleteData(
                table: "MakerSpaceProjects",
                keyColumn: "Id",
                keyValue: 24);

            migrationBuilder.DeleteData(
                table: "MakerSpaceProjects",
                keyColumn: "Id",
                keyValue: 26);

            migrationBuilder.DeleteData(
                table: "MakerSpaceProjects",
                keyColumn: "Id",
                keyValue: 27);

            migrationBuilder.DeleteData(
                table: "MakerSpaceProjects",
                keyColumn: "Id",
                keyValue: 28);

            migrationBuilder.DeleteData(
                table: "MakerSpaceProjects",
                keyColumn: "Id",
                keyValue: 29);

            migrationBuilder.DeleteData(
                table: "MakerSpaceProjects",
                keyColumn: "Id",
                keyValue: 30);

            migrationBuilder.DeleteData(
                table: "MakerSpaceProjects",
                keyColumn: "Id",
                keyValue: 34);

            migrationBuilder.DeleteData(
                table: "MakerSpaceProjects",
                keyColumn: "Id",
                keyValue: 35);

            migrationBuilder.DeleteData(
                table: "MakerSpaceProjects",
                keyColumn: "Id",
                keyValue: 38);

            migrationBuilder.DeleteData(
                table: "MakerSpaceProjects",
                keyColumn: "Id",
                keyValue: 41);

            migrationBuilder.DeleteData(
                table: "MakerSpaceProjects",
                keyColumn: "Id",
                keyValue: 42);

            migrationBuilder.DeleteData(
                table: "MakerSpaceProjects",
                keyColumn: "Id",
                keyValue: 44);

            migrationBuilder.DeleteData(
                table: "MakerSpaceProjects",
                keyColumn: "Id",
                keyValue: 46);

            migrationBuilder.DeleteData(
                table: "MakerSpaceProjects",
                keyColumn: "Id",
                keyValue: 47);

            migrationBuilder.DeleteData(
                table: "MakerSpaceProjects",
                keyColumn: "Id",
                keyValue: 49);

            migrationBuilder.DeleteData(
                table: "MakerSpaceProjects",
                keyColumn: "Id",
                keyValue: 65);

            migrationBuilder.DeleteData(
                table: "MakerSpaceProjects",
                keyColumn: "Id",
                keyValue: 68);

            migrationBuilder.DeleteData(
                table: "MakerSpaceProjects",
                keyColumn: "Id",
                keyValue: 71);

            migrationBuilder.DeleteData(
                table: "MakerSpaceProjects",
                keyColumn: "Id",
                keyValue: 72);

            migrationBuilder.DeleteData(
                table: "MakerSpaceProjects",
                keyColumn: "Id",
                keyValue: 76);

            migrationBuilder.DeleteData(
                table: "MakerSpaceProjects",
                keyColumn: "Id",
                keyValue: 78);

            migrationBuilder.DeleteData(
                table: "MakerSpaceProjects",
                keyColumn: "Id",
                keyValue: 84);

            migrationBuilder.DeleteData(
                table: "MakerSpaceProjects",
                keyColumn: "Id",
                keyValue: 86);

            migrationBuilder.DeleteData(
                table: "MakerSpaceProjects",
                keyColumn: "Id",
                keyValue: 89);

            migrationBuilder.DeleteData(
                table: "MakerSpaceProjects",
                keyColumn: "Id",
                keyValue: 90);

            migrationBuilder.DeleteData(
                table: "MakerSpaceProjects",
                keyColumn: "Id",
                keyValue: 93);

            migrationBuilder.DeleteData(
                table: "MakerSpaceProjects",
                keyColumn: "Id",
                keyValue: 96);

            migrationBuilder.DeleteData(
                table: "MakerSpaceProjects",
                keyColumn: "Id",
                keyValue: 100);

            migrationBuilder.DeleteData(
                table: "MakerSpaceProjects",
                keyColumn: "Id",
                keyValue: 104);

            migrationBuilder.DeleteData(
                table: "MakerSpaceProjects",
                keyColumn: "Id",
                keyValue: 105);

            migrationBuilder.DeleteData(
                table: "MakerSpaceProjects",
                keyColumn: "Id",
                keyValue: 106);

            migrationBuilder.DeleteData(
                table: "MakerSpaceProjects",
                keyColumn: "Id",
                keyValue: 107);

            migrationBuilder.DeleteData(
                table: "MakerSpaceProjects",
                keyColumn: "Id",
                keyValue: 108);

            migrationBuilder.DeleteData(
                table: "MakerSpaceProjects",
                keyColumn: "Id",
                keyValue: 109);

            migrationBuilder.DeleteData(
                table: "MakerSpaceProjects",
                keyColumn: "Id",
                keyValue: 110);

            migrationBuilder.DeleteData(
                table: "MakerSpaceProjects",
                keyColumn: "Id",
                keyValue: 111);

            migrationBuilder.DeleteData(
                table: "MakerSpaceProjects",
                keyColumn: "Id",
                keyValue: 112);

            migrationBuilder.DeleteData(
                table: "MakerSpaceProjects",
                keyColumn: "Id",
                keyValue: 113);

            migrationBuilder.DeleteData(
                table: "MakerSpaceProjects",
                keyColumn: "Id",
                keyValue: 114);

            migrationBuilder.DeleteData(
                table: "MakerSpaceProjects",
                keyColumn: "Id",
                keyValue: 116);

            migrationBuilder.DeleteData(
                table: "MakerSpaceProjects",
                keyColumn: "Id",
                keyValue: 117);

            migrationBuilder.DeleteData(
                table: "MakerSpaceProjects",
                keyColumn: "Id",
                keyValue: 118);

            migrationBuilder.DeleteData(
                table: "MakerSpaceProjects",
                keyColumn: "Id",
                keyValue: 119);

            migrationBuilder.DeleteData(
                table: "MakerSpaceProjects",
                keyColumn: "Id",
                keyValue: 120);

            migrationBuilder.DeleteData(
                table: "MakerSpaceProjects",
                keyColumn: "Id",
                keyValue: 121);

            migrationBuilder.DeleteData(
                table: "MakerSpaceProjects",
                keyColumn: "Id",
                keyValue: 122);

            migrationBuilder.DeleteData(
                table: "MakerSpaceProjects",
                keyColumn: "Id",
                keyValue: 123);

            migrationBuilder.DeleteData(
                table: "MakerSpaceProjects",
                keyColumn: "Id",
                keyValue: 124);

            migrationBuilder.DeleteData(
                table: "MakerSpaceProjects",
                keyColumn: "Id",
                keyValue: 125);

            migrationBuilder.DeleteData(
                table: "MakerSpaceProjects",
                keyColumn: "Id",
                keyValue: 126);

            migrationBuilder.DeleteData(
                table: "MakerSpaceProjects",
                keyColumn: "Id",
                keyValue: 127);

            migrationBuilder.DeleteData(
                table: "MakerSpaceProjects",
                keyColumn: "Id",
                keyValue: 128);

            migrationBuilder.DeleteData(
                table: "MakerSpaceProjects",
                keyColumn: "Id",
                keyValue: 129);

            migrationBuilder.DeleteData(
                table: "MakerSpaceProjects",
                keyColumn: "Id",
                keyValue: 130);

            migrationBuilder.DeleteData(
                table: "MakerSpaceProjects",
                keyColumn: "Id",
                keyValue: 131);

            migrationBuilder.DeleteData(
                table: "MakerSpaceProjects",
                keyColumn: "Id",
                keyValue: 132);

            migrationBuilder.DeleteData(
                table: "MakerSpaceProjects",
                keyColumn: "Id",
                keyValue: 133);

            migrationBuilder.DeleteData(
                table: "MakerSpaceProjects",
                keyColumn: "Id",
                keyValue: 134);

            migrationBuilder.DeleteData(
                table: "MakerSpaceProjects",
                keyColumn: "Id",
                keyValue: 135);

            migrationBuilder.DeleteData(
                table: "MakerSpaceProjects",
                keyColumn: "Id",
                keyValue: 136);

            migrationBuilder.DeleteData(
                table: "MakerSpaceProjects",
                keyColumn: "Id",
                keyValue: 137);

            migrationBuilder.DeleteData(
                table: "MakerSpaceProjects",
                keyColumn: "Id",
                keyValue: 138);

            migrationBuilder.DeleteData(
                table: "MakerSpaceProjects",
                keyColumn: "Id",
                keyValue: 139);

            migrationBuilder.DeleteData(
                table: "MakerSpaceProjects",
                keyColumn: "Id",
                keyValue: 140);

            migrationBuilder.DeleteData(
                table: "MakerSpaceProjects",
                keyColumn: "Id",
                keyValue: 141);

            migrationBuilder.DeleteData(
                table: "MakerSpaceProjects",
                keyColumn: "Id",
                keyValue: 142);

            migrationBuilder.DeleteData(
                table: "MakerSpaceProjects",
                keyColumn: "Id",
                keyValue: 143);

            migrationBuilder.DeleteData(
                table: "MakerSpaceProjects",
                keyColumn: "Id",
                keyValue: 144);

            migrationBuilder.DeleteData(
                table: "MakerSpaceProjects",
                keyColumn: "Id",
                keyValue: 145);

            migrationBuilder.DeleteData(
                table: "PromptAiSettings",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "PromptTypeGuidances",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "PromptTypeGuidances",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "PromptTypeGuidances",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "PromptTypeGuidances",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "PromptTypeGuidances",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "PromptTypeGuidances",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "PromptTypeGuidances",
                keyColumn: "Id",
                keyValue: 7);

            migrationBuilder.DropColumn(
                name: "FilterSystemPreamble",
                table: "PromptAiSettings");

            migrationBuilder.DropColumn(
                name: "KiAssistantSystemPrompt",
                table: "PromptAiSettings");

            migrationBuilder.DropColumn(
                name: "SmartSelectionSystemPreamble",
                table: "PromptAiSettings");

            migrationBuilder.DropColumn(
                name: "SmartSelectionUserPrompt",
                table: "PromptAiSettings");

            migrationBuilder.UpdateData(
                table: "Events",
                keyColumn: "Id",
                keyValue: 1,
                column: "Termin",
                value: new DateTime(2025, 10, 21, 0, 0, 0, 0, DateTimeKind.Local));

            migrationBuilder.UpdateData(
                table: "MakerSpaceProjects",
                keyColumn: "Id",
                keyValue: 6,
                columns: new[] { "ImageUrl", "Tags" },
                values: new object[] { "~/images/makerspace/03acee10-9669-4563-a960-74de0d9fcb63.jpg", "Bilder" });

            migrationBuilder.UpdateData(
                table: "MakerSpaceProjects",
                keyColumn: "Id",
                keyValue: 12,
                column: "Tags",
                value: "Color");

            migrationBuilder.UpdateData(
                table: "MakerSpaceProjects",
                keyColumn: "Id",
                keyValue: 16,
                column: "Tags",
                value: "Forschung");

            migrationBuilder.UpdateData(
                table: "MakerSpaceProjects",
                keyColumn: "Id",
                keyValue: 22,
                column: "Tags",
                value: "Schriften");

            migrationBuilder.UpdateData(
                table: "MakerSpaceProjects",
                keyColumn: "Id",
                keyValue: 25,
                column: "Tags",
                value: "Visualisieren,Design");

            migrationBuilder.UpdateData(
                table: "MakerSpaceProjects",
                keyColumn: "Id",
                keyValue: 31,
                column: "Tags",
                value: "Color,Design");

            migrationBuilder.UpdateData(
                table: "MakerSpaceProjects",
                keyColumn: "Id",
                keyValue: 32,
                columns: new[] { "ImageUrl", "Tags" },
                values: new object[] { "/images/makerspace/d040eb27-3c73-4688-a473-4aaa88347d5d.jpg", "Programmieren" });

            migrationBuilder.UpdateData(
                table: "MakerSpaceProjects",
                keyColumn: "Id",
                keyValue: 33,
                column: "Tags",
                value: "H5P");

            migrationBuilder.UpdateData(
                table: "MakerSpaceProjects",
                keyColumn: "Id",
                keyValue: 36,
                column: "Tags",
                value: "Forschung");

            migrationBuilder.UpdateData(
                table: "MakerSpaceProjects",
                keyColumn: "Id",
                keyValue: 39,
                column: "Tags",
                value: "Forschung");

            migrationBuilder.UpdateData(
                table: "MakerSpaceProjects",
                keyColumn: "Id",
                keyValue: 40,
                columns: new[] { "ImageUrl", "Tags" },
                values: new object[] { "/images/makerspace/f4f8cd0b-6490-4953-8eac-5cb026fc77be.png", "Lehre planen" });

            migrationBuilder.UpdateData(
                table: "MakerSpaceProjects",
                keyColumn: "Id",
                keyValue: 43,
                columns: new[] { "ImageUrl", "Tags" },
                values: new object[] { "/images/makerspace/06b291e5-cb3c-4bc1-9649-f85b916f6a1a.jpg", "Bilder" });

            migrationBuilder.UpdateData(
                table: "MakerSpaceProjects",
                keyColumn: "Id",
                keyValue: 45,
                column: "Tags",
                value: "Übersetzungen");

            migrationBuilder.UpdateData(
                table: "MakerSpaceProjects",
                keyColumn: "Id",
                keyValue: 48,
                column: "Tags",
                value: "Visualisieren");

            migrationBuilder.UpdateData(
                table: "MakerSpaceProjects",
                keyColumn: "Id",
                keyValue: 50,
                column: "Tags",
                value: "Audio,Video");

            migrationBuilder.UpdateData(
                table: "MakerSpaceProjects",
                keyColumn: "Id",
                keyValue: 51,
                column: "Tags",
                value: "Plagiatprüfung");

            migrationBuilder.UpdateData(
                table: "MakerSpaceProjects",
                keyColumn: "Id",
                keyValue: 52,
                columns: new[] { "ImageUrl", "Tags" },
                values: new object[] { "/images/makerspace/31af52ba-c50b-40f8-9c25-ef5113f4b39a.jpg", "Bilder,Video,Musik,3D-Objekte,Multimodal" });

            migrationBuilder.UpdateData(
                table: "MakerSpaceProjects",
                keyColumn: "Id",
                keyValue: 53,
                columns: new[] { "ImageUrl", "Tags" },
                values: new object[] { "/images/makerspace/fe9b4c0f-19d1-4273-8853-55a69cd21f1f.jpg", "Programmieren" });

            migrationBuilder.UpdateData(
                table: "MakerSpaceProjects",
                keyColumn: "Id",
                keyValue: 54,
                columns: new[] { "Description", "ImageUrl", "Tags" },
                values: new object[] { "<p>Die App ist eine KI-gestützte Plattform, die Bilder in surreal-verzerrte Kunstwerke verwandelt, indem sie Muster und Strukturen verstärkt.</p>", "/images/makerspace/f00b78a2-2a89-4013-8de5-6a6d012d9015.jpg", "Bilder" });

            migrationBuilder.UpdateData(
                table: "MakerSpaceProjects",
                keyColumn: "Id",
                keyValue: 55,
                column: "Tags",
                value: "Textanalyse,Schreibprozesse");

            migrationBuilder.UpdateData(
                table: "MakerSpaceProjects",
                keyColumn: "Id",
                keyValue: 56,
                column: "Tags",
                value: "Bilder");

            migrationBuilder.UpdateData(
                table: "MakerSpaceProjects",
                keyColumn: "Id",
                keyValue: 57,
                column: "Tags",
                value: "Forschung");

            migrationBuilder.UpdateData(
                table: "MakerSpaceProjects",
                keyColumn: "Id",
                keyValue: 58,
                columns: new[] { "ImageUrl", "Tags" },
                values: new object[] { "/images/makerspace/4078c720-f28a-4eca-9e25-11fbbb940f95.jpg", "Musik" });

            migrationBuilder.UpdateData(
                table: "MakerSpaceProjects",
                keyColumn: "Id",
                keyValue: 59,
                column: "Tags",
                value: "Plagiatprüfung");

            migrationBuilder.UpdateData(
                table: "MakerSpaceProjects",
                keyColumn: "Id",
                keyValue: 60,
                column: "Tags",
                value: "Sprachgeneratoren");

            migrationBuilder.UpdateData(
                table: "MakerSpaceProjects",
                keyColumn: "Id",
                keyValue: 61,
                column: "Tags",
                value: "Forschung,Zitationen");

            migrationBuilder.UpdateData(
                table: "MakerSpaceProjects",
                keyColumn: "Id",
                keyValue: 62,
                column: "Tags",
                value: "Forschung");

            migrationBuilder.UpdateData(
                table: "MakerSpaceProjects",
                keyColumn: "Id",
                keyValue: 63,
                column: "Tags",
                value: "Forschung,Literaturrecherche");

            migrationBuilder.UpdateData(
                table: "MakerSpaceProjects",
                keyColumn: "Id",
                keyValue: 64,
                column: "Tags",
                value: "Visualisieren");

            migrationBuilder.UpdateData(
                table: "MakerSpaceProjects",
                keyColumn: "Id",
                keyValue: 66,
                column: "Tags",
                value: "Schreibprozesse");

            migrationBuilder.UpdateData(
                table: "MakerSpaceProjects",
                keyColumn: "Id",
                keyValue: 67,
                column: "Tags",
                value: "Forschung,Literaturrecherche");

            migrationBuilder.UpdateData(
                table: "MakerSpaceProjects",
                keyColumn: "Id",
                keyValue: 69,
                columns: new[] { "Tags", "Top" },
                values: new object[] { "Video", true });

            migrationBuilder.UpdateData(
                table: "MakerSpaceProjects",
                keyColumn: "Id",
                keyValue: 70,
                columns: new[] { "Tags", "Top" },
                values: new object[] { "Design", true });

            migrationBuilder.UpdateData(
                table: "MakerSpaceProjects",
                keyColumn: "Id",
                keyValue: 73,
                columns: new[] { "ImageUrl", "Tags" },
                values: new object[] { "/images/makerspace/87d5a4c5-6cba-4417-ad34-1409f486ce2c.png", "Bilder,Prompt" });

            migrationBuilder.UpdateData(
                table: "MakerSpaceProjects",
                keyColumn: "Id",
                keyValue: 74,
                columns: new[] { "Description", "ImageUrl", "Tags" },
                values: new object[] { "<p>Immersity AI verwandelt mit KI 2D-Bilder und -Videos in bewegende 3D-Erlebnisse. Nutzer können ihre Inhalte in 3D-Motion-Bilder, 3D-Videos oder 3D-Bilder umwandeln und sie auf XR-Geräten wie Apple Vision Pro und Meta Quest erleben.</p>", "/images/makerspace/90ca7f27-c02e-4278-8a4a-ee72ea405e1a.png", "Video,Bilder" });

            migrationBuilder.UpdateData(
                table: "MakerSpaceProjects",
                keyColumn: "Id",
                keyValue: 75,
                column: "Tags",
                value: "Quiz,Prüfungen,Feedback");

            migrationBuilder.UpdateData(
                table: "MakerSpaceProjects",
                keyColumn: "Id",
                keyValue: 77,
                column: "Tags",
                value: "Textanalyse,Schreibprozesse");

            migrationBuilder.UpdateData(
                table: "MakerSpaceProjects",
                keyColumn: "Id",
                keyValue: 79,
                columns: new[] { "Tags", "events" },
                values: new object[] { "Logo,Design", true });

            migrationBuilder.UpdateData(
                table: "MakerSpaceProjects",
                keyColumn: "Id",
                keyValue: 80,
                columns: new[] { "ImageUrl", "Tags" },
                values: new object[] { "/images/makerspace/72a374b1-337c-40b4-8c15-57177cd3ccfa.jpg", "Bilder,Video" });

            migrationBuilder.UpdateData(
                table: "MakerSpaceProjects",
                keyColumn: "Id",
                keyValue: 81,
                column: "Tags",
                value: "Literaturrecherche,Textanalyse");

            migrationBuilder.UpdateData(
                table: "MakerSpaceProjects",
                keyColumn: "Id",
                keyValue: 83,
                column: "Tags",
                value: "Forschung,Textanalyse");

            migrationBuilder.UpdateData(
                table: "MakerSpaceProjects",
                keyColumn: "Id",
                keyValue: 85,
                column: "Tags",
                value: "Forschung");

            migrationBuilder.UpdateData(
                table: "MakerSpaceProjects",
                keyColumn: "Id",
                keyValue: 87,
                columns: new[] { "Tags", "events" },
                values: new object[] { "Bilder", true });

            migrationBuilder.UpdateData(
                table: "MakerSpaceProjects",
                keyColumn: "Id",
                keyValue: 88,
                column: "Tags",
                value: "Visualisieren");

            migrationBuilder.UpdateData(
                table: "MakerSpaceProjects",
                keyColumn: "Id",
                keyValue: 91,
                column: "Tags",
                value: "Visualisieren,Design");

            migrationBuilder.UpdateData(
                table: "MakerSpaceProjects",
                keyColumn: "Id",
                keyValue: 92,
                column: "Tags",
                value: "Sprachgeneratoren");

            migrationBuilder.UpdateData(
                table: "MakerSpaceProjects",
                keyColumn: "Id",
                keyValue: 94,
                column: "Tags",
                value: "Visualisieren");

            migrationBuilder.UpdateData(
                table: "MakerSpaceProjects",
                keyColumn: "Id",
                keyValue: 95,
                column: "Tags",
                value: "Schreibprozesse");

            migrationBuilder.UpdateData(
                table: "MakerSpaceProjects",
                keyColumn: "Id",
                keyValue: 97,
                column: "Tags",
                value: "Transkription");

            migrationBuilder.UpdateData(
                table: "MakerSpaceProjects",
                keyColumn: "Id",
                keyValue: 98,
                columns: new[] { "Forschung", "Tags", "events" },
                values: new object[] { true, "Literaturrecherche,Textanalyse", true });

            migrationBuilder.UpdateData(
                table: "MakerSpaceProjects",
                keyColumn: "Id",
                keyValue: 99,
                columns: new[] { "Tags", "events" },
                values: new object[] { "Bilder", true });

            migrationBuilder.UpdateData(
                table: "MakerSpaceProjects",
                keyColumn: "Id",
                keyValue: 102,
                column: "Tags",
                value: "Schreibprozesse,Plagiatprüfung");

            migrationBuilder.UpdateData(
                table: "MakerSpaceProjects",
                keyColumn: "Id",
                keyValue: 103,
                columns: new[] { "ImageUrl", "Tags" },
                values: new object[] { "/images/makerspace/79336b20-f6e1-4cef-9205-8d326b659a61.png", "Prüfungen,Quiz" });

            migrationBuilder.UpdateData(
                table: "MakerSpaceProjects",
                keyColumn: "Id",
                keyValue: 115,
                column: "Tags",
                value: "Schreibassistent");

            migrationBuilder.InsertData(
                table: "MakerSpaceProjects",
                columns: new[] { "Id", "Beitraege", "Description", "DisplayOrder", "Forschung", "ITRecht", "ImageUrl", "ProjectUrl", "Tags", "Title", "Top", "download", "events", "lesezeichen", "netzwerk", "tutorial" },
                values: new object[,]
                {
                    { 1001, false, "<p>Seelab ist eine KI-Plattform zur schnellen Erstellung hochwertiger, markenkonformer Produktbilder und Videos in 4K. Nutzer trainieren eigene Markenmodelle und bearbeiten Visuals einfach. Ideal für Marken, Agenturen und Kreativteams, mit sicherem, datenschutzkonformem Service.</p>", 1001, false, false, "/images/makerspace/a2147749-4a7c-44a6-8532-12dad25ffaa9.png", "https://www.seelab.ai/en/", "Produktbilder,Video", "Seelab", true, false, true, false, false, false },
                    { 1002, false, "<p>Pollo AI ist eine Plattform zur Erstellung von KI-generierten Bildern und Videos. Sie wandelt Texte, Bilder oder Videos in animierte Clips in verschiedenen Stilen um. Zudem bietet sie Tools wie Upscaling, Face-Swap und Stiltransfers. Ideal ist sie für Content Creator, die schnell visuelle Inhalte produzieren möchten.</p>", 1002, false, false, "/images/makerspace/64de651e-1b58-4557-bbd1-8ffff56057f9.png", "https://pollo.ai/", "Video", "Pollo AI", true, false, true, false, false, false },
                    { 1008, false, "<p><strong>Bundesministerium der Justiz &nbsp;- März 2024</strong><br>Der Text behandelt den Schutz des Urheberrechts im Zusammenhang mit Künstlicher Intelligenz. Er erklärt, dass beim Training von KI urheberrechtlich geschützte Inhalte nur mit Zustimmung genutzt werden dürfen. Reine KI-generierte Inhalte sind nicht urheberrechtlich geschützt - es sei denn, der Mensch wirkt kreativ mit. Die EU-KI-Verordnung soll den Schutz durch Transparenzpflichten stärken. Außerdem wird auf Deepfakes eingegangen, die Urheber- oder Persönlichkeitsrechte verletzen können.</p>", 1008, false, false, "/images/makerspace/b60ab4cc-5ab8-4550-a808-0a6472c249dc.jpg", "https://www.bmjv.de/SharedDocs/Downloads/DE/Themen/Nav_Themen/240305_FAQ_KI_Urheberrecht.pdf?__blob=publicationFile&v=2", "", "Künstliche Intelligenz und Urheberrecht - Fragen und Antworten", false, false, false, false, false, false },
                    { 1009, false, "<p>Die Webseite <a href=https://ai-act-law.eu/de/>ai-act-law.eu/de</a> bietet den vollständigen deutschen Text der EU-KI-Verordnung (AI Act) 2024/1689, die ab August 2026 gilt. Die Verordnung legt harmonisierte Regeln für KI-Systeme fest, insbesondere für Hochrisiko-Anwendungen mit strengeren Anforderungen an Dokumentation, Risikomanagement und Transparenz. Anbieter und Betreiber müssen sich mit diesen Pflichten vertraut machen. Die Seite ermöglicht eine übersichtliche Navigation durch die Verordnung und dient als wichtige Informationsquelle zu den rechtlichen Vorgaben für KI in der EU.</p>", 1009, false, false, "/images/makerspace/2402351d-adac-4487-8f3c-18b4e8647141.jpg", "https://ai-act-law.eu/de/", "", "Die KI-Verordnung von ", false, false, false, false, false, false },
                    { 1010, false, "<p>Das Papier behandelt die Frage, wie das Urheberrecht auf Inhalte anzuwenden ist, die mithilfe Künstlicher Intelligenz (KI) erstellt wurden, und beleuchtet dabei sowohl die Herausforderungen als auch die Chancen für den Bildungsbereich. KI ist zunehmend Teil unseres Alltags - beispielsweise durch Sprachassistenten oder Programme, die automatisch Texte und Bilder erzeugen. Besonders im schulischen Kontext eröffnen sich dadurch neue Möglichkeiten, etwa bei der Erstellung von Unterrichtsmaterialien oder der Nutzung und Entwicklung von Open Educational Resources (OER). Gleichzeitig entsteht jedoch ein erhöhter Bedarf an rechtlicher Orientierung: Lehrkräfte müssen lernen, wie sie mit urheberrechtlichen Fragen im Zusammenhang mit KI-generierten Inhalten verantwortungsvoll umgehen.</p>", 1010, false, false, "/images/makerspace/d0bacbc1-f0c5-44c8-969d-1676575f8918.jpg", "https://www.iit-berlin.de/wp-content/uploads/2025/02/2025-03-iit-perspektive-Urheberrecht_KI_Beitraege_V05.pdf", "", "Urheberrecht bei KI-generierten Beiträgen: Handlungsbedarfe und Nutzungschancen für den Bildungskontext (Working Paper des Instituts für Innovation und Technik (iit) Nr. 78)", false, false, false, false, false, false },
                    { 1011, false, "<p>Das Werk gibt einen kompakten Überblick über zentrale rechtliche Fragen zur Nutzung generativer KI an Hochschulen in NRW. Es behandelt Urheberrechte beim KI-Training und bei der Inhaltserstellung, beleuchtet Persönlichkeitsrechte und Datenschutz und geht auch auf arbeitsrechtliche Aspekte im Hochschulkontext ein.</p>", 1011, false, false, "/images/makerspace/43d85f27-f10e-4046-b1bb-3a23e71a523b.jpg", "https://kiconnect.pages.rwth-aachen.de/pages/download/docs/040_Rechtsfragen_Allgemein_KIconnectNRW_aktuell.pdf", "", "Allgemeine Rechtsfragen zum Umgang mit generativen KI-Diensten in der Version 1.0 von 2024-07-10 (Ki:content.NRW)", false, false, false, false, false, false },
                    { 1012, false, "<p>Dieser Leitfaden zeigt auf, wie textgenerierende KI-Systeme wie ChatGPT im schulischen Kontext sinnvoll genutzt werden können. Er erklärt, wie diese Systeme funktionieren, beleuchtet rechtliche und praktische Rahmenbedingungen für ihren Einsatz im Unterricht und thematisiert die damit verbundenen Chancen und Herausforderungen für Lehrkräfte und Schüler*innen sowie weitere relevante Aspekte.</p>", 1012, false, false, "/images/makerspace/a7adde30-7cc6-4a85-acaf-4ff73a075033.jpg", "https://www.schulministerium.nrw/system/files/media/document/file/handlungsleitfaden_ki_msb_nrw_230223.pdf", "", "Umgang mit textgenerierenden KI-Systemen - Ein Handlungsleitfaden (Ministerium für Schule und Bildung NRW)", false, false, false, false, false, false },
                    { 1013, false, "<p>Die Empfehlungen zeigen auf, wie Künstliche Intelligenz (KI) an der Universität Duisburg-Essen (UDE) sinnvoll in Studium und Lehre integriert werden kann. Sie behandeln Chancen und Grenzen von KI-Tools im Hinblick auf Texte, Urheberrecht und Datenschutz, legen den Fokus auf die Förderung von KI-Kompetenzen bei Studierenden und thematisieren den Umgang mit KI in Prüfungen und wissenschaftlichem Arbeiten - inklusive neuer Prüfungsformate.</p>", 1013, false, false, "/images/makerspace/733ebd42-c0d6-42cd-a1ad-fe3ce5df8435.jpg", "https://www.uni-due.de/imperia/md/content/e-learning/strategie/ki_in_studium_und_lehre_-_empfehlungen_zum_umgang_an_der_ude_v1.0.pdf", "", "KÜNSTLICHE INTELLIGENZ IN STUDIUM UND LEHRE - Empfehlungen zum Umgang an der UDE (Uni Duisburg Essen)", false, false, false, false, false, false },
                    { 1014, false, "<p>Dieser Text untersucht zentrale urheberrechtliche Fragestellungen im Zusammenhang mit Künstlicher Intelligenz und maschinellem Lernen. Er erläutert die rechtliche Einordnung von Trainingsdaten und Lizenzierungen, analysiert einschlägige Ausnahmen wie das Text- und Data-Mining und geht auf den Schutz von Algorithmen, Software und KI-generierten Inhalten ein.</p>", 1014, false, false, "/images/makerspace/7529792d-fdcd-42e9-b25f-3dd0ae864274.jpg", "https://www.bundestag.de/resource/blob/592106/74cd41f0bd7bc5684f6defaade176515/wd-10-067-18-pdf-data.pdf", "", "Künstliche Intelligenz und Machine Learning - Eine urheberrechtliche Betrachtung - Deutscher Bundestag WD 10 - 3000 - 67/18", false, false, false, false, false, false },
                    { 1015, false, "<p>Diese Handreichung bietet praktische Hinweise zum Einsatz generativer KI im Studium. Sie zeigt, was erlaubt ist, wie KI-Tools sinnvoll eingesetzt werden können und welche Risiken zu beachten sind. Im Fokus stehen klare Absprachen mit Lehrenden, der unterstützende Einsatz von KI beim Lernen, Schreiben und Organisieren sowie die Notwendigkeit einer kritischen Reflexion, da KI nicht immer verlässliche Informationen liefert.</p>", 1015, false, false, "/images/makerspace/464fa0ec-f359-4e3d-aab0-4464bd297063.jpg", "https://lehre-virtuell.uni-frankfurt.de/knowhow/einsatz-von-generativer-ki-im-studium-handlungsempfehlungen-fuer-studierende/", "", "Einsatz von generativer KI im Studium - Handlungsempfehlungen für Studierend (Goethe Universität Frankfurt am Main) ", false, false, false, false, false, false },
                    { 1018, false, "<p>Die Ausarbeitung gibt einen umfassenden Überblick über urheberrechtliche Fragestellungen im Kontext des Trainings generativer KI-Modelle. Es behandelt die rechtliche Bewertung der Nutzung urheberrechtlich geschützer Werke als Trainingsdaten, insbesondere im Hinblick auf die urheberrechtlichen Schranken, wie Text- und Data Mining, sowie auf Fragen der Vervielfältigung und der Schutzfähigkeit von KI-generierten Werken. Zudem werden Risiken im Zusammenhang mit der möglichen Memorierung geschützter Inhalte, die zu Lizenzverletzungen führen könnten, analysiert. Das Dokument diskutiert weiterhin die rechtlichen Unsicherheiten bei der Verwendung von Trainingsdaten, die mit bestehenden und geplanten europäischen Regelungen, wie der KI-Verordnung, verknüpft sind. Insgesamt bietet es eine differenzierte Betrachtung der Herausforderung, Innovation im KI-Bereich mit gesetzlichen Vorgaben in Einklang zu bringen.</p>", 1018, false, false, "/images/makerspace/bf1a0099-ca9c-4193-870c-6af535339820.jpg", "https://www.nomos-elibrary.de/de/10.5771/9783748949558/urheberrecht-und-training-generativer-ki-modelle?page=1", "", "Urheberrecht und Training generativer KI-Modelle", false, false, false, false, false, false },
                    { 1019, false, "<p>PromptBase ist ein Online-Marktplatz, auf dem Nutzer KI-Prompts kaufen und verkaufen können - für Tools wie ChatGPT, Midjourney, DALL·E, Stable Diffusion u.v.m. Dort findest du über 200.000 kuratierte Prompts von Expert:innen</p>", 1019, false, false, "/images/makerspace/834b6192-2ad2-4c27-80cc-d00b52db90c7.jpg", "https://promptbase.com/", "Prompt", " PromptBase", false, false, false, false, false, false },
                    { 1020, false, "<p>Sora ist ein von OpenAI entwickeltes KI-Modell für Text-zu-Video. Es kann moderne Kurzvideos direkt aus textuellen Beschreibungen erstellen und bestehende Videos verlängern oder neu bearbeiten.</p>", 1020, false, false, "/images/makerspace/64b475bb-2e29-4d4f-a0fc-bba7f5176a9b.jpg", "https://sora.chatgpt.com/", "Bilder,Video", "Sora", false, false, false, false, false, false },
                    { 1021, false, "<p>Die <strong>Academic Cloud</strong> ist das zentrale Portal für Universitäten, Hochschulen und Forschungseinrichtungen. Nach dem Login erhalten Nutzende Zugriff auf digitale Speicherplätze, gemeinsame Dateibearbeitung (inkl. LaTeX), Umfrage-Tools, PIDs für Publikationen, Kurz-URL-Generatoren sowie Chat- und Videokonferenzräume. Ein zentrales Angebot ist <strong>Chat AI</strong> - ein einfacher und sicherer Zugang zu leistungsstarker generativer KI. Über die intuitive Oberfläche können Nutzende direkt mit verschiedenen KI-Modellen chatten und so Forschung, Studium und Lehre effizient unterstützen.</p>", 1021, true, false, "/images/makerspace/95a56719-db66-4374-beec-17ce58cd5e80.jpg", "https://academiccloud.de/", "LLM-basierten Systeme", "Academinc Cloud", true, false, false, false, false, false },
                    { 1022, false, "<p>Die Veröffentlichung von Schmohl, Watanabe und Schelling (2023) gibt einen umfassenden Überblick über die Integration von Künstlicher Intelligenz (KI) in der Hochschulbildung. Er beleuchtet verschiedene Einsatzmöglichkeiten wie Learning Analytics, individualisierte Lernmaterialien und die Unterstützung forschenden Lernens. Dabei werden sowohl Chancen als auch Herausforderungen, insbesondere ethische Aspekte, Akzeptanzfragen und Transparenz von KI-Systemen, ausführlich diskutiert. Der Beitrag liefert wertvolle Impulse für die zukünftige Gestaltung von Lehre und Lernen mit KI.</p><p><strong>Autoren:</strong><br>Schmohl, Tobias [Hrsg.]; Watanabe, Alice [Hrsg.]; Schelling, Kathrin [Hrsg.]<br><i>Künstliche Intelligenz in der Hochschulbildung. Chancen und Grenzen des KI-gestützten Lernens und Lehrens.</i><br>Bielefeld: transcript 2023, 283 S. - (Hochschulbildung: Lehre und Forschung; 4)</p>", 1022, false, false, "/images/makerspace/5ee86348-21f5-4a88-a794-c6fa63612700.jpg", "https://www.pedocs.de/volltexte/2023/26427/pdf/Schmohl_Watanabe_Schelling_2023_Kuenstliche_Intelligenz.pdf", "", "Künstliche Intelligenz in der Hochschulbildung", false, false, false, false, false, false },
                    { 1023, false, "<p>Die Webseite der Bundesnetzagentur informiert umfassend über Künstliche Intelligenz (KI) in den von ihr regulierten Bereichen wie Telekommunikation, Energie und Post. Sie bietet Unterstützung bei der Umsetzung der EU-KI-Verordnung und fördert den verantwortungsvollen Einsatz von KI. Zudem organisiert die Agentur Formate wie das KI-Café, um den Austausch zwischen verschiedenen Akteuren zu ermöglichen und aktuelle Themen zu diskutieren.</p>", 1023, false, false, "/images/makerspace/ddd4317f-ad9b-4e03-a792-1c91e8a7914d.jpg", "https://www.bundesnetzagentur.de/DE/Fachthemen/Digitales/KI/start_ki.html", "", "Umsetzung der EU-KI-Verordnung", false, false, false, false, false, false },
                    { 1024, false, "<p><strong>Künstliche Intelligenz: Forschung am Fraunhofer IKS </ strong > auf der Fraunhofer Institute - Website gibt einen umfassenden Überblick,was das Institut im Bereich KI - Forschung anbietet:</ p > ", 1024, false, false, "/images/makerspace/d1e9e9cc-74d5-47c2-bbb5-b973ee311de0.jpg", "https://www.iks.fraunhofer.de/de/themen/kuenstliche-intelligenz/kuenstliche-intelligenz-forschung.html", "", "Künstliche Intelligenz: Forschung am Fraunhofer IKS", false, false, false, false, false, false },
                    { 1025, false, "<p>Die Fraunhofer-IKS-Seite <strong>Künstliche Intelligenz in der Medizin</strong> zeigt, wie KI-Technologien sicher und praxisnah im Gesundheitswesen eingesetzt werden können. Sie stellt Projekte vor wie generative KI für Wissensmanagement, intelligente Personaleinsatzplanung, datengestützte Frühversorgung von Frühgeborenen und Frameworks zur rechtssicheren Validierung medizinischer KI-Systeme - stets mit Fokus auf Sicherheit, Effizienz und Patientennutzen.</p>", 1025, false, false, "/images/makerspace/383a014e-c714-4991-8f91-5743c10e9ffe.jpg", "https://www.iks.fraunhofer.de/de/themen/kuenstliche-intelligenz/kuenstliche-intelligenz-medizin.html", "", "Künstliche Intelligenz in der Medizin", false, false, false, false, false, false },
                    { 1026, false, "<p>Der Beitrag beschreibt innovative didaktische Ansätze für digitale Lernangebote am Beispiel des <strong>KI-Campus</strong>. Es zeigt, wie Online-Plattformen durch interaktive, flexible und personalisierte Lernformate gezielt KI-Kompetenzen fördern können, und gibt Empfehlungen für zukunftsfähige digitale Bildungsformate.</p>", 1026, false, false, "/images/makerspace/9ed1aaf6-adcb-448c-b764-854ec799382c.jpg", "https://link.springer.com/chapter/10.1007/978-3-658-32849-8_34", "", "Zukunftsfähige Formate für digitale Lernangebote", false, false, false, false, false, false },
                    { 1027, false, "<p>Der Beitrag beschreibt die Entwicklung des regelbasierten Chatbots <strong>OSABot</strong> an der Technischen Hochschule Nürnberg. Ziel ist es, Studienanfänger:innen schon zu Beginn ihres Studiums bei der Organisation des Lernens und beim Aufbau von study skills zu unterstützen. Grundlage bildet ein psychologisch fundierter Test zur Studierfähigkeit, aus dem der Chatbot personalisierte Tipps - etwa zu Zeitplanung oder Lernmethoden - ableitet. Durch dialogische Interaktionen soll er zugleich Motivation und Selbstreflexion fördern. Nach einer Erprobungsphase ist der dauerhafte Einsatz vorgesehen.</p>", 1027, false, false, "/images/makerspace/ddbe3235-8420-4b35-8cc4-a8f20a2af012.jpg", "https://www.pedocs.de/volltexte/2023/27835/pdf/Helten_et_al_2023_Wie_kann_ich_dich_unterstuetzen.pdf", "", "Wie kann ich dich unterstützen?. Chatbot-basierte Lernunterstützung für Studienanfänger:innen", false, false, false, false, false, false },
                    { 1030, false, "<p>Das <strong>Rechtsgutachten von Prof. Dr. Thomas Hoeren</strong> analysiert die Bedeutung der europäischen KI-Verordnung für Hochschulen. Es klärt, dass das Wissenschaftsprivileg nur für rein forschungsbezogene KI-Systeme gilt, während bei Praxiseinsatz die Verordnung ab Inbetriebnahme greift. Hochschulen müssen KI-Kompetenz bei Beschäftigten sicherstellen, bei Studierenden nur bei geforderter Nutzung; Schulungen sind flexibel, aber verbindlich. Hochschulen sind meist Betreiber, selten Anbieter von KI-Systemen, wobei Anbieterstatus durch Entwicklung oder wesentliche Anpassung entsteht. Hochrisiko-Anwendungen, wie Bewertung von Lernergebnissen, unterliegen strengen Vorgaben, außer bei unverbindlichem Feedback. Learning Analytics sind nur bei komplexen, anpassungsfähigen Systemen KI-Systeme. Open-Source-KI hat begrenzte Ausnahmen, da Hochrisiko-Systeme reguliert bleiben. Das Gutachten, entstanden im Projekt KI:edu.nrw, bietet Hochschulen Orientierung für den rechtskonformen Umgang mit der KI-Verordnung.</p>", 1030, false, false, "/images/makerspace/0bac6777-d88e-436a-8aa4-a6a5f18324d0.jpg", "https://www.itm.nrw/wp-content/uploads/2025/08/KI-edu-nrw_Rechtsgutachten-zur-Bedeutung-der-europaeischen-KI-Verordnung-fuer-Hochschulen.pdf", "", "Rechtsgutachten zur Bedeutung der europäischen KI-Verordnung für Hochschulen", false, false, false, false, false, false },
                    { 1031, false, "<p>Erstellt komplette Songs mit Gesang und Instrumenten aus einem Textprompt.</p>", 1031, false, false, "/images/makerspace/beae188a-492e-42ea-992d-7edf95e3f954.jpg", "https://www.udio.com/", "Musik", "Udio", false, false, false, false, false, false },
                    { 1032, false, "<p>Wandelt Texte in fertige Songs um (inkl. Vocals).</p>", 1032, false, false, "/images/makerspace/04f1aee9-290b-4313-8cdf-5a41a6bdab97.jpg", "https://suno.com/home", "Musik", "Suno AI", false, false, false, false, false, false },
                    { 1033, false, "<p>Der Einstieg: Stil wählen ␦ Song generieren - fertig! Erstellte Songs können auch auf Spotify veröffentlicht werden.</p>", 1033, false, false, "/images/makerspace/37bd0eda-645b-4c84-af3b-31e80302e3ec.jpg", "https://boomy.com", "Musik", "Boomy", false, false, false, false, false, false },
                    { 1034, false, "<p>Erstellt automatisch passende Hintergrundmusik in unterschiedlichen Stilen. Ist geeignet für private Projekte und Social Media Clips.<br>Für professionelle, kommerzielle Einsätze ist ein kostenpflichtiges Abo nötig.</p>", 1034, false, false, "/images/makerspace/28064572-2e46-4738-be24-eb4e2f194ad1.jpg", "https://mubert.com/", "Musik", "Mubert", false, false, false, false, false, false },
                    { 1035, false, "<p>Erzeugt Musik aus Text-Eingaben, indem Klangbilder in hörbare Sounds umgewandelt werden. Die Ergebnisse eignen sich gut für kreative Ideen (Open-Source).</p>", 1035, false, false, "/images/makerspace/fa093163-49eb-45fa-8080-187a31bad9f5.jpg", "https://www.producer.ai/", "Musik", "Riffusion", false, false, false, false, false, false },
                    { 1036, false, "<p>Bietet die Generierung von Tracks in verschiedenen Genres.</p>", 1036, false, false, "/images/makerspace/f20bdeaa-814b-488b-952c-1a14b89d28c2.jpg", "https://www.loudly.com", "Musik", "Loudly", false, false, false, false, false, false },
                    { 1037, false, "<p>Die Website stellt den <strong>Prompt Report</strong> vor - eine umfassende Analyse von Methoden des Prompt-Engineerings für generative KI. Darin werden eine Klassifikation von 58 Prompting-Ansätzen für Sprachmodelle sowie 40 zusätzliche Varianten beschrieben. Außerdem enthält der Bericht Empfehlungen und Leitlinien für erfolgreiches Prompting, inklusive praxisnaher Hinweise für moderne Systeme wie ChatGPT. Ziel der Untersuchung ist es, Begriffe zu präzisieren und ein klares, systematisches Verständnis zu vermitteln.</p>", 1037, true, false, "/images/makerspace/dbc6ba03-eb42-446f-ae03-e22e986f67a1.jpg", "https://arxiv.org/pdf/2406.06608", "", "Studie: Prompt Report", false, false, false, false, false, false },
                    { 1038, false, "<p>Emergent ist ein agentic vibe-coding Tool, das es erlaubt, vollständig funktionsfähige, produktionsbereite Anwendungen aus einfachen Textanweisungen zu erstellen - ganz ohne selbst zu programmieren.</p>", 1038, false, false, "/images/makerspace/c96f823e-d96c-4e8e-af84-443bc4473637.jpg", "https://app.emergent.sh/", "Programmieren", "Emergent", false, false, false, false, false, false },
                    { 1039, false, "<p><strong>Poe</strong> ist ein von Quora entwickelter Dienst, der den Zugang zu unterschiedlichen Sprach-KI-Modellen über eine zentrale Plattform bündelt. Nutzerinnen und Nutzer können damit je nach Bedarf zwischen verschiedenen Assistenten wechseln und ihre Stärken vergleichen. Poe ermöglicht die Erstellung eigener Bots, die auf vorhandenen Modellen aufsetzen und individuell angepasst werden können.</p>", 1039, false, false, "/images/makerspace/4627846d-0649-46c5-8e60-df89db3b6f63.jpg", "https://poe.com/", "LLM-basierten Systeme", "Poe", false, false, false, false, false, false },
                    { 1040, false, "<p>Google AI Studio ist eine Plattform von Google, die den Zugang zu großen Sprachmodellen wie Gemini ermöglicht. Nutzer können damit KI-gestützte Anwendungen erstellen, Texte analysieren oder generative Inhalte erzeugen. Die Plattform dient als Schnittstelle zwischen Entwicklern und den zugrundeliegenden LLM-Technologien und erleichtert die Integration in eigene Projekte.</p>", 1040, false, false, "/images/makerspace/20364079-f5f6-4bbd-b6c9-e4472f3a139f.jpg", "https://aistudio.google.com/", "LLM-basierten Systeme", "Google AI Studio", false, false, false, false, false, false },
                    { 1041, false, "<p>Grok ist ein Large Language Model, das von Meta entwickelt wurde und für Textverarbeitung, Konversation und kreative Inhalte eingesetzt wird. Es kann komplexe Texte verstehen, zusammenfassen und neue Inhalte generieren.</p>", 1041, false, false, "/images/makerspace/057243d6-c790-46dc-8b1e-0a28eaece4c7.jpg", "https://grok.com/", "LLM-basierten Systeme", "Grok", false, false, false, false, false, false },
                    { 1042, false, "<p>Microsoft Copilot ist ein KI-Assistent, der in Office-Anwendungen und den Edge-Browser integriert ist und auf LLM-Technologie basiert. Er unterstützt beim Schreiben, Analysieren und Zusammenfassen von Texten sowie bei Automatisierungen. Copilot nutzt dabei große Sprachmodelle, um produktive Arbeitsprozesse zu vereinfachen.</p>", 1042, false, false, "/images/makerspace/0a1cc7cc-170e-4986-aa50-4fc665ade4f1.jpg", "https://copilot.microsoft.com/", "LLM-basierten Systeme", "Microsoft Copilot", false, false, false, false, false, false },
                    { 1043, false, "<p>Claude ist ein von Anthropic entwickeltes Large Language Model, das natürliche Sprache versteht und generiert. Es wird für Chatbots, Textanalysen und kreative Inhalte eingesetzt. Nutzer können Claude über verschiedene Plattformen in Anwendungen integrieren, um KI-gestützte Aufgaben effizient zu erledigen.</p>", 1043, false, false, "/images/makerspace/72469555-768c-4533-8ad3-df2e3e7ee24b.jpg", "https://claude.ai/", "LLM-basierten Systeme", "Claude", false, false, false, false, false, false },
                    { 1044, false, "<p>ChatGPT ist ein KI-gestützter Chatbot von OpenAI, der auf großen Sprachmodellen basiert und natürliche Unterhaltungen führen kann. Er unterstützt beim Schreiben, Recherchieren und Beantworten komplexer Fragen. ChatGPT wird über Web, Apps oder APIs in verschiedenen Anwendungen integriert.</p>", 1044, false, false, "/images/makerspace/749c6a03-7d60-408a-b43b-14fb093ccb90.jpg", "https://chatgpt.com/", "LLM-basierten Systeme", "ChatGPT", false, false, false, false, false, false },
                    { 1045, false, "<p>Mistral ist ein Large Language Model, das für Textgenerierung und -verarbeitung entwickelt wurde. Es kann komplexe Texte verstehen, zusammenfassen und eigenständig Inhalte erstellen. Mistral wird sowohl direkt als Modell als auch über Plattformen genutzt, die auf LLM-Technologie basieren.</p>", 1045, false, false, "/images/makerspace/3b49c9c7-b80e-4102-ad72-051c5a43e49f.jpg", "https://mistral.ai/", "LLM-basierten Systeme", "Mistral", false, false, false, false, false, false },
                    { 1046, false, "<p>DeepSpeek ist eine KI-Anwendung, die sich auf Sprachverarbeitung und Text-zu-Sprache-Funktionen spezialisiert hat. Sie ermöglicht realistische Sprachausgabe und kann gesprochene Inhalte analysieren oder generieren. Die Plattform wird häufig für Kommunikation, Lernanwendungen und Content-Erstellung eingesetzt.</p>", 1046, false, false, "/images/makerspace/4a5328e9-1b6b-4617-ad54-f60eaee1389f.jpg", "https://www.deepseek.com/", "LLM-basierten Systeme", "DeepSpeek", false, false, false, false, false, false },
                    { 1047, false, "<p><strong>TinyWow</strong> ist eine webbasierte Plattform, die eine Vielzahl von <strong>kostenlosen Online-Tools</strong> anbietet.&nbsp;</p><p>Dazu gehören u. a.:</p><ul><li><strong>PDF-Tools</strong> (komprimieren, konvertieren, zusammenführen)</li><li><strong>Video- und Bildbearbeitung</strong> (konvertieren, zuschneiden, komprimieren)</li><li><strong>Dateikonvertierung</strong> (Word  PDF, Video  MP3 usw.)</li><li><strong>Online-Utilities</strong> wie Screenshots, Meme-Generatoren, Texttools</li></ul>", 1047, false, false, "/images/makerspace/4f7fa365-8fcd-42a5-94ee-0623ec5a2274.jpg", "https://tinywow.com/", "Multimodal", "Tinywow", false, false, false, false, false, false },
                    { 1048, false, "<p><strong>Remove.photos </strong>ist eine App zum schnellen Entfernen von Bildhintergründen. Es erstellt automatisch transparente PNGs und ermöglicht das Ersetzen oder Bearbeiten von Hintergründen.</p><p><strong>Kernfunktionen:</strong></p><p>Automatisches Entfernen von Bildhintergründen Erstellen von transparenten PNG-Bildern Einfache Bildbearbeitung (neuen Hintergrund einsetzen, Bild aufhellen) Entfernen von Vordergrundobjekten Kostenfrei und ohne Registrierung nutzbar</p>", 1048, false, false, "/images/makerspace/cc022bab-fc56-4285-8cca-ca2b07a0c426.jpg", "https://remove.photos/", "Bilder", "Remove.Photos", false, false, false, false, false, false },
                    { 1049, false, "<p>TidyText.cc ist ein kostenloses Online-Tool zur Bereinigung von Textausgaben aus KI-Generatoren wie ChatGPT. Es entfernt automatisch unerwünschte Formatierungen wie Markdown, HTML-Tags, Fußnoten oder Sonderzeichen. Der Text wird in ein sauberes, strukturiertes Format umgewandelt, das sich direkt in Google Docs oder Microsoft Word einfügen lässt.</p><p><strong>Kernfunktionen:</strong></p><ul><li>Automatisches Entfernen von Markdown- und HTML-Formatierungen</li><li>Bereinigung von Fußnoten und Sonderzeichen</li><li>Umwandlung in sauberes, strukturiertes Textformat</li><li>Kostenlose Nutzung ohne Registrierung</li></ul>", 1049, false, false, "/images/makerspace/9edd47b4-fd90-430f-abc3-b7f9e40b025d.jpg", "https://tidytext.cc/", "Plagiatprüfung,Schreibprozesse", "TidyText.cc", false, false, false, false, false, false },
                    { 1050, false, "<p><strong>SoSciSurvey </strong>- View Chars ist ein kostenloses Online-Tool zur Erkennung unsichtbarer oder nicht druckbarer Zeichen in Texten. Es identifiziert versteckte Unicode-Zeichen wie Leerzeichen, Steuerzeichen oder Formatierungen, die beim Kopieren und Einfügen entstehen können. Die Darstellung der Zeichen in Unicode ermöglicht eine genaue Analyse und erleichtert die Bereinigung von Texten, etwa für Online-Umfragen oder Datenverarbeitung.</p><p><strong>Kernfunktionen:</strong></p><ul><li>Erkennung von unsichtbaren und nicht druckbaren Unicode-Zeichen</li><li>Identifikation von Steuerzeichen und versteckten Formatierungen</li><li>Unterstützung bei der Textbereinigung für Datenanalyse</li><li>Kostenlos nutzbar, keine Registrierung erforderlich</li></ul><p>Dieses Tool ist besonders nützlich für Entwickler und Autoren, die mit Textdaten arbeiten und sicherstellen möchten, dass ihre Daten frei von unsichtbaren Störungen sind.</p>", 1050, false, false, "/images/makerspace/ef5526ea-b7ab-4f7c-8345-7cc89e378c73.jpg", "https://www.soscisurvey.de/tools/view-chars.php", "Plagiatprüfung,Schreibprozesse", "Soscisurvey", false, false, false, false, false, false }
                });

            migrationBuilder.UpdateData(
                table: "PortalVideo",
                keyColumn: "Id",
                keyValue: 1,
                column: "UploadDate",
                value: new DateTime(2025, 10, 14, 14, 1, 2, 368, DateTimeKind.Local).AddTicks(6480));

            migrationBuilder.UpdateData(
                table: "Skills",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2025, 10, 14, 14, 1, 2, 368, DateTimeKind.Local).AddTicks(6259));
        }
    }
}
