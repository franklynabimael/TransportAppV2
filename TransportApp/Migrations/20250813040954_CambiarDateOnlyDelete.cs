﻿using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TransportApp.Migrations
{
    /// <inheritdoc />
    public partial class CambiarDateOnlyDelete : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "HoraSalida",
                table: "Viajes",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(TimeOnly),
                oldType: "time");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<TimeOnly>(
                name: "HoraSalida",
                table: "Viajes",
                type: "time",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");
        }
    }
}
