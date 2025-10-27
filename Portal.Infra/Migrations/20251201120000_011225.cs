using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GestaoSaudeIdosos.Infra.Migrations
{
    /// <inheritdoc />
    public partial class _011225 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Nome",
                table: "Usuarios",
                newName: "NomeCompleto");

            migrationBuilder.RenameColumn(
                name: "Nome",
                table: "Pacientes",
                newName: "NomeCompleto");

            migrationBuilder.AddColumn<string>(
                name: "CpfRg",
                table: "Usuarios",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ImagemPerfil",
                table: "Usuarios",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CondicoesCronicas",
                table: "Pacientes",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CpfRg",
                table: "Pacientes",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DietasRestricoes",
                table: "Pacientes",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "HistoricoCirurgico",
                table: "Pacientes",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ImagemPerfil",
                table: "Pacientes",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MobilidadeAuxilios",
                table: "Pacientes",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RiscoQuedas",
                table: "Pacientes",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CpfRg",
                table: "Usuarios");

            migrationBuilder.DropColumn(
                name: "ImagemPerfil",
                table: "Usuarios");

            migrationBuilder.DropColumn(
                name: "CondicoesCronicas",
                table: "Pacientes");

            migrationBuilder.DropColumn(
                name: "CpfRg",
                table: "Pacientes");

            migrationBuilder.DropColumn(
                name: "DietasRestricoes",
                table: "Pacientes");

            migrationBuilder.DropColumn(
                name: "HistoricoCirurgico",
                table: "Pacientes");

            migrationBuilder.DropColumn(
                name: "ImagemPerfil",
                table: "Pacientes");

            migrationBuilder.DropColumn(
                name: "MobilidadeAuxilios",
                table: "Pacientes");

            migrationBuilder.DropColumn(
                name: "RiscoQuedas",
                table: "Pacientes");

            migrationBuilder.RenameColumn(
                name: "NomeCompleto",
                table: "Usuarios",
                newName: "Nome");

            migrationBuilder.RenameColumn(
                name: "NomeCompleto",
                table: "Pacientes",
                newName: "Nome");
        }
    }
}
