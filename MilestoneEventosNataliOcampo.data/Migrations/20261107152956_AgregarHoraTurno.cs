using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MilestoneEventosNataliOcampo.data.Migrations
{
    /// <inheritdoc />
    public partial class AgregarHoraTurno : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<TimeOnly>(
                name: "HoraTurno",
                table: "Turnos",
                type: "time",
                nullable: false,
                defaultValue: new TimeOnly(0, 0, 0));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "HoraTurno",
                table: "Turnos");
        }
    }
}
