using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProjectsWebApp.DataAccsess.Migrations
{
    /// <inheritdoc />
    public partial class ReaddMultiImages : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PromptImage_PromptTemplate_PromptTemplateId",
                table: "PromptImage");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PromptImage",
                table: "PromptImage");

            migrationBuilder.RenameTable(
                name: "PromptImage",
                newName: "PromptImages");

            migrationBuilder.RenameIndex(
                name: "IX_PromptImage_PromptTemplateId",
                table: "PromptImages",
                newName: "IX_PromptImages_PromptTemplateId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_PromptImages",
                table: "PromptImages",
                column: "Id");

            migrationBuilder.UpdateData(
                table: "PortalVideo",
                keyColumn: "Id",
                keyValue: 1,
                column: "UploadDate",
                value: new DateTime(2025, 11, 26, 12, 19, 44, 697, DateTimeKind.Local).AddTicks(7871));

            migrationBuilder.UpdateData(
                table: "PromptAiSettings",
                keyColumn: "Id",
                keyValue: 1,
                column: "UpdatedAt",
                value: new DateTime(2025, 11, 26, 11, 19, 44, 697, DateTimeKind.Utc).AddTicks(7323));

            migrationBuilder.UpdateData(
                table: "PromptTypeGuidances",
                keyColumn: "Id",
                keyValue: 1,
                column: "UpdatedAt",
                value: new DateTime(2025, 11, 26, 11, 19, 44, 697, DateTimeKind.Utc).AddTicks(7349));

            migrationBuilder.UpdateData(
                table: "PromptTypeGuidances",
                keyColumn: "Id",
                keyValue: 2,
                column: "UpdatedAt",
                value: new DateTime(2025, 11, 26, 11, 19, 44, 697, DateTimeKind.Utc).AddTicks(7350));

            migrationBuilder.UpdateData(
                table: "PromptTypeGuidances",
                keyColumn: "Id",
                keyValue: 3,
                column: "UpdatedAt",
                value: new DateTime(2025, 11, 26, 11, 19, 44, 697, DateTimeKind.Utc).AddTicks(7351));

            migrationBuilder.UpdateData(
                table: "PromptTypeGuidances",
                keyColumn: "Id",
                keyValue: 4,
                column: "UpdatedAt",
                value: new DateTime(2025, 11, 26, 11, 19, 44, 697, DateTimeKind.Utc).AddTicks(7351));

            migrationBuilder.UpdateData(
                table: "PromptTypeGuidances",
                keyColumn: "Id",
                keyValue: 5,
                column: "UpdatedAt",
                value: new DateTime(2025, 11, 26, 11, 19, 44, 697, DateTimeKind.Utc).AddTicks(7352));

            migrationBuilder.UpdateData(
                table: "PromptTypeGuidances",
                keyColumn: "Id",
                keyValue: 6,
                column: "UpdatedAt",
                value: new DateTime(2025, 11, 26, 11, 19, 44, 697, DateTimeKind.Utc).AddTicks(7353));

            migrationBuilder.UpdateData(
                table: "PromptTypeGuidances",
                keyColumn: "Id",
                keyValue: 7,
                column: "UpdatedAt",
                value: new DateTime(2025, 11, 26, 11, 19, 44, 697, DateTimeKind.Utc).AddTicks(7354));

            migrationBuilder.UpdateData(
                table: "Skills",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 26, 12, 19, 44, 697, DateTimeKind.Local).AddTicks(7581));

            migrationBuilder.AddForeignKey(
                name: "FK_PromptImages_PromptTemplate_PromptTemplateId",
                table: "PromptImages",
                column: "PromptTemplateId",
                principalTable: "PromptTemplate",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PromptImages_PromptTemplate_PromptTemplateId",
                table: "PromptImages");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PromptImages",
                table: "PromptImages");

            migrationBuilder.RenameTable(
                name: "PromptImages",
                newName: "PromptImage");

            migrationBuilder.RenameIndex(
                name: "IX_PromptImages_PromptTemplateId",
                table: "PromptImage",
                newName: "IX_PromptImage_PromptTemplateId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_PromptImage",
                table: "PromptImage",
                column: "Id");

            migrationBuilder.UpdateData(
                table: "PortalVideo",
                keyColumn: "Id",
                keyValue: 1,
                column: "UploadDate",
                value: new DateTime(2025, 11, 26, 12, 16, 0, 168, DateTimeKind.Local).AddTicks(2232));

            migrationBuilder.UpdateData(
                table: "PromptAiSettings",
                keyColumn: "Id",
                keyValue: 1,
                column: "UpdatedAt",
                value: new DateTime(2025, 11, 26, 11, 16, 0, 168, DateTimeKind.Utc).AddTicks(1625));

            migrationBuilder.UpdateData(
                table: "PromptTypeGuidances",
                keyColumn: "Id",
                keyValue: 1,
                column: "UpdatedAt",
                value: new DateTime(2025, 11, 26, 11, 16, 0, 168, DateTimeKind.Utc).AddTicks(1654));

            migrationBuilder.UpdateData(
                table: "PromptTypeGuidances",
                keyColumn: "Id",
                keyValue: 2,
                column: "UpdatedAt",
                value: new DateTime(2025, 11, 26, 11, 16, 0, 168, DateTimeKind.Utc).AddTicks(1655));

            migrationBuilder.UpdateData(
                table: "PromptTypeGuidances",
                keyColumn: "Id",
                keyValue: 3,
                column: "UpdatedAt",
                value: new DateTime(2025, 11, 26, 11, 16, 0, 168, DateTimeKind.Utc).AddTicks(1656));

            migrationBuilder.UpdateData(
                table: "PromptTypeGuidances",
                keyColumn: "Id",
                keyValue: 4,
                column: "UpdatedAt",
                value: new DateTime(2025, 11, 26, 11, 16, 0, 168, DateTimeKind.Utc).AddTicks(1657));

            migrationBuilder.UpdateData(
                table: "PromptTypeGuidances",
                keyColumn: "Id",
                keyValue: 5,
                column: "UpdatedAt",
                value: new DateTime(2025, 11, 26, 11, 16, 0, 168, DateTimeKind.Utc).AddTicks(1658));

            migrationBuilder.UpdateData(
                table: "PromptTypeGuidances",
                keyColumn: "Id",
                keyValue: 6,
                column: "UpdatedAt",
                value: new DateTime(2025, 11, 26, 11, 16, 0, 168, DateTimeKind.Utc).AddTicks(1659));

            migrationBuilder.UpdateData(
                table: "PromptTypeGuidances",
                keyColumn: "Id",
                keyValue: 7,
                column: "UpdatedAt",
                value: new DateTime(2025, 11, 26, 11, 16, 0, 168, DateTimeKind.Utc).AddTicks(1660));

            migrationBuilder.UpdateData(
                table: "Skills",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 26, 12, 16, 0, 168, DateTimeKind.Local).AddTicks(1941));

            migrationBuilder.AddForeignKey(
                name: "FK_PromptImage_PromptTemplate_PromptTemplateId",
                table: "PromptImage",
                column: "PromptTemplateId",
                principalTable: "PromptTemplate",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
