using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WatchTowerBackend.Migrations
{
    /// <inheritdoc />
    public partial class NameIntoCameraName : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Name",
                table: "Cameras",
                newName: "CameraName");

            migrationBuilder.RenameIndex(
                name: "IX_Cameras_Name",
                table: "Cameras",
                newName: "IX_Cameras_CameraName");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "CameraName",
                table: "Cameras",
                newName: "Name");

            migrationBuilder.RenameIndex(
                name: "IX_Cameras_CameraName",
                table: "Cameras",
                newName: "IX_Cameras_Name");
        }
    }
}
