using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TransportApp.Migrations
{
    /// <inheritdoc />
    public partial class CambiarDateOnly : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FechaCreacion",
                table: "Viajes");

            migrationBuilder.AlterColumn<TimeOnly>(
                name: "HoraSalida",
                table: "Viajes",
                type: "time",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "HoraSalida",
                table: "Viajes",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(TimeOnly),
                oldType: "time");

            migrationBuilder.AddColumn<DateTime>(
                name: "FechaCreacion",
                table: "Viajes",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }
    }
}
