using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MobileMonitoringBackend.Migrations
{
    /// <inheritdoc />
    public partial class ChangesAfterConsultation : Migration
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
                name: "RoomId",
                table: "Rooms");

            migrationBuilder.DropColumn(
                name: "RoomId",
                table: "Cameras");

            migrationBuilder.RenameColumn(
                name: "CameraId",
                table: "Cameras",
                newName: "CameraToken");

            migrationBuilder.AddColumn<string>(
                name: "RoomName",
                table: "Rooms",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "OwnerLogin",
                table: "Rooms",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "AcceptationState",
                table: "Cameras",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RoomName",
                table: "Cameras",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "RoomName1",
                table: "Cameras",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Rooms",
                table: "Rooms",
                column: "RoomName");

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Login = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Password = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Login);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Rooms_OwnerLogin",
                table: "Rooms",
                column: "OwnerLogin");

            migrationBuilder.CreateIndex(
                name: "IX_Cameras_RoomName1",
                table: "Cameras",
                column: "RoomName1");

            migrationBuilder.AddForeignKey(
                name: "FK_Cameras_Rooms_RoomName1",
                table: "Cameras",
                column: "RoomName1",
                principalTable: "Rooms",
                principalColumn: "RoomName");

            migrationBuilder.AddForeignKey(
                name: "FK_Rooms_Users_OwnerLogin",
                table: "Rooms",
                column: "OwnerLogin",
                principalTable: "Users",
                principalColumn: "Login",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Cameras_Rooms_RoomName1",
                table: "Cameras");

            migrationBuilder.DropForeignKey(
                name: "FK_Rooms_Users_OwnerLogin",
                table: "Rooms");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Rooms",
                table: "Rooms");

            migrationBuilder.DropIndex(
                name: "IX_Rooms_OwnerLogin",
                table: "Rooms");

            migrationBuilder.DropIndex(
                name: "IX_Cameras_RoomName1",
                table: "Cameras");

            migrationBuilder.DropColumn(
                name: "RoomName",
                table: "Rooms");

            migrationBuilder.DropColumn(
                name: "OwnerLogin",
                table: "Rooms");

            migrationBuilder.DropColumn(
                name: "AcceptationState",
                table: "Cameras");

            migrationBuilder.DropColumn(
                name: "RoomName",
                table: "Cameras");

            migrationBuilder.DropColumn(
                name: "RoomName1",
                table: "Cameras");

            migrationBuilder.RenameColumn(
                name: "CameraToken",
                table: "Cameras",
                newName: "CameraId");

            migrationBuilder.AddColumn<Guid>(
                name: "RoomId",
                table: "Rooms",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "RoomId",
                table: "Cameras",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Rooms",
                table: "Rooms",
                column: "RoomId");

            migrationBuilder.CreateIndex(
                name: "IX_Cameras_RoomId",
                table: "Cameras",
                column: "RoomId");

            migrationBuilder.AddForeignKey(
                name: "FK_Cameras_Rooms_RoomId",
                table: "Cameras",
                column: "RoomId",
                principalTable: "Rooms",
                principalColumn: "RoomId");
        }
    }
}
