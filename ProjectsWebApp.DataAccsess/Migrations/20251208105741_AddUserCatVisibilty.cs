using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace ProjectsWebApp.DataAccsess.Migrations
{
    /// <inheritdoc />
    public partial class AddUserCatVisibilty : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "UserFilterCategoryVisibilities",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<string>(type: "text", nullable: false),
                    FilterCategoryId = table.Column<int>(type: "integer", nullable: false),
                    IsHidden = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserFilterCategoryVisibilities", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserFilterCategoryVisibilities_FilterCategories_FilterCateg~",
                        column: x => x.FilterCategoryId,
                        principalTable: "FilterCategories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserSecurityStates",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<string>(type: "text", nullable: false),
                    LockoutWindowCount = table.Column<int>(type: "integer", nullable: false),
                    IsPermanentlyLocked = table.Column<bool>(type: "boolean", nullable: false),
                    FirstLockoutAtUtc = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    LastLockoutAtUtc = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserSecurityStates", x => x.Id);
                });

            migrationBuilder.UpdateData(
                table: "Events",
                keyColumn: "Id",
                keyValue: 1,
                column: "Termin",
                value: new DateTime(2025, 12, 15, 0, 0, 0, 0, DateTimeKind.Local));

            migrationBuilder.UpdateData(
                table: "PortalVideo",
                keyColumn: "Id",
                keyValue: 1,
                column: "UploadDate",
                value: new DateTime(2025, 12, 8, 11, 57, 40, 124, DateTimeKind.Local).AddTicks(4306));

            migrationBuilder.UpdateData(
                table: "PromptAiSettings",
                keyColumn: "Id",
                keyValue: 1,
                column: "UpdatedAt",
                value: new DateTime(2025, 12, 8, 10, 57, 40, 124, DateTimeKind.Utc).AddTicks(3758));

            migrationBuilder.UpdateData(
                table: "PromptTypeGuidances",
                keyColumn: "Id",
                keyValue: 1,
                column: "UpdatedAt",
                value: new DateTime(2025, 12, 8, 10, 57, 40, 124, DateTimeKind.Utc).AddTicks(3816));

            migrationBuilder.UpdateData(
                table: "PromptTypeGuidances",
                keyColumn: "Id",
                keyValue: 2,
                column: "UpdatedAt",
                value: new DateTime(2025, 12, 8, 10, 57, 40, 124, DateTimeKind.Utc).AddTicks(3818));

            migrationBuilder.UpdateData(
                table: "PromptTypeGuidances",
                keyColumn: "Id",
                keyValue: 3,
                column: "UpdatedAt",
                value: new DateTime(2025, 12, 8, 10, 57, 40, 124, DateTimeKind.Utc).AddTicks(3819));

            migrationBuilder.UpdateData(
                table: "PromptTypeGuidances",
                keyColumn: "Id",
                keyValue: 4,
                column: "UpdatedAt",
                value: new DateTime(2025, 12, 8, 10, 57, 40, 124, DateTimeKind.Utc).AddTicks(3820));

            migrationBuilder.UpdateData(
                table: "PromptTypeGuidances",
                keyColumn: "Id",
                keyValue: 5,
                column: "UpdatedAt",
                value: new DateTime(2025, 12, 8, 10, 57, 40, 124, DateTimeKind.Utc).AddTicks(3820));

            migrationBuilder.UpdateData(
                table: "PromptTypeGuidances",
                keyColumn: "Id",
                keyValue: 6,
                column: "UpdatedAt",
                value: new DateTime(2025, 12, 8, 10, 57, 40, 124, DateTimeKind.Utc).AddTicks(3821));

            migrationBuilder.UpdateData(
                table: "PromptTypeGuidances",
                keyColumn: "Id",
                keyValue: 7,
                column: "UpdatedAt",
                value: new DateTime(2025, 12, 8, 10, 57, 40, 124, DateTimeKind.Utc).AddTicks(3822));

            migrationBuilder.UpdateData(
                table: "Skills",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2025, 12, 8, 11, 57, 40, 124, DateTimeKind.Local).AddTicks(4024));

            migrationBuilder.CreateIndex(
                name: "IX_UserFilterCategoryVisibilities_FilterCategoryId",
                table: "UserFilterCategoryVisibilities",
                column: "FilterCategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_UserFilterCategoryVisibilities_UserId_FilterCategoryId",
                table: "UserFilterCategoryVisibilities",
                columns: new[] { "UserId", "FilterCategoryId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserSecurityStates_UserId",
                table: "UserSecurityStates",
                column: "UserId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserFilterCategoryVisibilities");

            migrationBuilder.DropTable(
                name: "UserSecurityStates");

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
        }
    }
}
