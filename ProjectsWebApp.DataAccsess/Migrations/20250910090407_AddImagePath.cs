using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProjectsWebApp.DataAccsess.Migrations
{
    /// <inheritdoc />
    public partial class AddImagePath : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "MakerSpaceProjects",
                keyColumn: "Id",
                keyValue: 6,
                column: "ImageUrl",
                value: "~/images/makerspace/03acee10-9669-4563-a960-74de0d9fcb63.jpg");

            migrationBuilder.UpdateData(
                table: "PortalVideo",
                keyColumn: "Id",
                keyValue: 1,
                column: "UploadDate",
                value: new DateTime(2025, 9, 10, 11, 4, 6, 457, DateTimeKind.Local).AddTicks(8675));

            migrationBuilder.UpdateData(
                table: "Skills",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2025, 9, 10, 11, 4, 6, 457, DateTimeKind.Local).AddTicks(8468));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "MakerSpaceProjects",
                keyColumn: "Id",
                keyValue: 6,
                column: "ImageUrl",
                value: "/images/makerspace/03acee10-9669-4563-a960-74de0d9fcb63.jpg");

            migrationBuilder.UpdateData(
                table: "PortalVideo",
                keyColumn: "Id",
                keyValue: 1,
                column: "UploadDate",
                value: new DateTime(2025, 9, 10, 10, 13, 29, 91, DateTimeKind.Local).AddTicks(3324));

            migrationBuilder.UpdateData(
                table: "Skills",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2025, 9, 10, 10, 13, 29, 91, DateTimeKind.Local).AddTicks(3061));
        }
    }
}
