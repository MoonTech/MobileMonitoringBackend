using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MobileMonitoringBackend.Migrations
{
    /// <inheritdoc />
    public partial class kjkl : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Cameras_Rooms_RoomName1",
                table: "Cameras");

            migrationBuilder.DropIndex(
                name: "IX_Cameras_RoomName1",
                table: "Cameras");

            migrationBuilder.DropColumn(
                name: "RoomName1",
                table: "Cameras");

            migrationBuilder.AlterColumn<string>(
                name: "RoomName",
                table: "Cameras",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

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

            migrationBuilder.DropIndex(
                name: "IX_Cameras_RoomName",
                table: "Cameras");

            migrationBuilder.AlterColumn<string>(
                name: "RoomName",
                table: "Cameras",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AddColumn<string>(
                name: "RoomName1",
                table: "Cameras",
                type: "nvarchar(450)",
                nullable: true);

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
        }
    }
}
