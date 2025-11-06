using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GestaoSaudeIdosos.Infra.Migrations
{
    /// <inheritdoc />
    public partial class Release_05 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Pacientes_FormularioId",
                table: "Pacientes");

            migrationBuilder.DropColumn(
                name: "FormularioId",
                table: "Pacientes");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "FormularioId",
                table: "Pacientes",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Pacientes_FormularioId",
                table: "Pacientes",
                column: "FormularioId");
        }
    }
}
