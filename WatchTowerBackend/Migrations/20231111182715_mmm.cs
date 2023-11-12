using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WatchTowerBackend.Migrations
{
    /// <inheritdoc />
    public partial class mmm : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Rooms_Users_OwnerLogin",
                table: "Rooms");

            migrationBuilder.RenameColumn(
                name: "OwnerLogin",
                table: "Rooms",
                newName: "OwnerId");

            migrationBuilder.RenameIndex(
                name: "IX_Rooms_OwnerLogin",
                table: "Rooms",
                newName: "IX_Rooms_OwnerId");

            migrationBuilder.AddForeignKey(
                name: "FK_Rooms_Users_OwnerId",
                table: "Rooms",
                column: "OwnerId",
                principalTable: "Users",
                principalColumn: "Login",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Rooms_Users_OwnerId",
                table: "Rooms");

            migrationBuilder.RenameColumn(
                name: "OwnerId",
                table: "Rooms",
                newName: "OwnerLogin");

            migrationBuilder.RenameIndex(
                name: "IX_Rooms_OwnerId",
                table: "Rooms",
                newName: "IX_Rooms_OwnerLogin");

            migrationBuilder.AddForeignKey(
                name: "FK_Rooms_Users_OwnerLogin",
                table: "Rooms",
                column: "OwnerLogin",
                principalTable: "Users",
                principalColumn: "Login",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
