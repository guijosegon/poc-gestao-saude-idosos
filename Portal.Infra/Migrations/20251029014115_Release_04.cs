using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GestaoSaudeIdosos.Infra.Migrations
{
    /// <inheritdoc />
    public partial class Release_04 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Configuracao",
                table: "Graficos",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "ExibirNoPortal",
                table: "Graficos",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "FormularioId",
                table: "Graficos",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Origem",
                table: "Graficos",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Tipo",
                table: "Graficos",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Graficos_FormularioId",
                table: "Graficos",
                column: "FormularioId");

            migrationBuilder.AddForeignKey(
                name: "FK_Graficos_Formularios_FormularioId",
                table: "Graficos",
                column: "FormularioId",
                principalTable: "Formularios",
                principalColumn: "FormularioId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Graficos_Formularios_FormularioId",
                table: "Graficos");

            migrationBuilder.DropIndex(
                name: "IX_Graficos_FormularioId",
                table: "Graficos");

            migrationBuilder.DropColumn(
                name: "Configuracao",
                table: "Graficos");

            migrationBuilder.DropColumn(
                name: "ExibirNoPortal",
                table: "Graficos");

            migrationBuilder.DropColumn(
                name: "FormularioId",
                table: "Graficos");

            migrationBuilder.DropColumn(
                name: "Origem",
                table: "Graficos");

            migrationBuilder.DropColumn(
                name: "Tipo",
                table: "Graficos");
        }
    }
}
