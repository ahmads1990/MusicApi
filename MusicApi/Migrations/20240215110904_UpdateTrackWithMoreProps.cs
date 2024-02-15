using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MusicApi.Migrations
{
    /// <inheritdoc />
    public partial class UpdateTrackWithMoreProps : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Length",
                table: "Tracks",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "ReleaseDate",
                table: "Tracks",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "TrackPath",
                table: "Tracks",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Length",
                table: "Tracks");

            migrationBuilder.DropColumn(
                name: "ReleaseDate",
                table: "Tracks");

            migrationBuilder.DropColumn(
                name: "TrackPath",
                table: "Tracks");
        }
    }
}
