using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WatchTowerBackend.Migrations
{
    /// <inheritdoc />
    public partial class AddCameraToken : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Cameras_CameraName",
                table: "Cameras");

            migrationBuilder.DropIndex(
                name: "IX_Cameras_RoomName",
                table: "Cameras");

            migrationBuilder.AddColumn<string>(
                name: "CameraToken",
                table: "Cameras",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_Cameras_RoomName_CameraName",
                table: "Cameras",
                columns: new[] { "RoomName", "CameraName" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Cameras_RoomName_CameraName",
                table: "Cameras");

            migrationBuilder.DropColumn(
                name: "CameraToken",
                table: "Cameras");

            migrationBuilder.CreateIndex(
                name: "IX_Cameras_CameraName",
                table: "Cameras",
                column: "CameraName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Cameras_RoomName",
                table: "Cameras",
                column: "RoomName");
        }
    }
}
