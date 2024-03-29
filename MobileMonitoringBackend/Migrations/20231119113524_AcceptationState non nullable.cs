﻿using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MobileMonitoringBackend.Migrations
{
    /// <inheritdoc />
    public partial class AcceptationStatenonnullable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<bool>(
                name: "AcceptationState",
                table: "Cameras",
                type: "bit",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldNullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<bool>(
                name: "AcceptationState",
                table: "Cameras",
                type: "bit",
                nullable: true,
                oldClrType: typeof(bool),
                oldType: "bit");
        }
    }
}
