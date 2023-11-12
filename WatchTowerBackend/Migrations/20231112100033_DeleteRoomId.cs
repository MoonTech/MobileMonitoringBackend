using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WatchTowerBackend.Migrations
{
    /// <inheritdoc />
    public partial class DeleteRoomId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Cameras_Rooms_RoomId",
                table: "Cameras");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Rooms",
                table: "Rooms");

            migrationBuilder.DropIndex(
                name: "IX_Cameras_RoomId",
                table: "Cameras");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "Rooms");

            migrationBuilder.DropColumn(
                name: "RoomId",
                table: "Cameras");

            migrationBuilder.AddColumn<string>(
                name: "RoomName",
                table: "Cameras",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Rooms",
                table: "Rooms",
                column: "RoomName");

            migrationBuilder.CreateIndex(
                name: "IX_Cameras_RoomName",
                table: "Cameras",
                column: "RoomName");

            migrationBuilder.AddForeignKey(
                name: "FK_Cameras_Rooms_RoomName",
                table: "Cameras",
                column: "RoomName",
                principalTable: "Rooms",
                principalColumn: "RoomName",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Cameras_Rooms_RoomName",
                table: "Cameras");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Rooms",
                table: "Rooms");

            migrationBuilder.DropIndex(
                name: "IX_Cameras_RoomName",
                table: "Cameras");

            migrationBuilder.DropColumn(
                name: "RoomName",
                table: "Cameras");

            migrationBuilder.AddColumn<Guid>(
                name: "Id",
                table: "Rooms",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "RoomId",
                table: "Cameras",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddPrimaryKey(
                name: "PK_Rooms",
                table: "Rooms",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_Cameras_RoomId",
                table: "Cameras",
                column: "RoomId");

            migrationBuilder.AddForeignKey(
                name: "FK_Cameras_Rooms_RoomId",
                table: "Cameras",
                column: "RoomId",
                principalTable: "Rooms",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
