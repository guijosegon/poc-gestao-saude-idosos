using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GestaoSaudeIdosos.Infra.Migrations
{
    /// <inheritdoc />
    public partial class Release_02 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Sexo",
                table: "Pacientes",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Sexo",
                table: "Pacientes");
        }
    }
}
