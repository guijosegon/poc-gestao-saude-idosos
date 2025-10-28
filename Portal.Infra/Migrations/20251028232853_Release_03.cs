using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace GestaoSaudeIdosos.Infra.Migrations
{
    /// <inheritdoc />
    public partial class Release_03 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FormularioCampoValores");

            migrationBuilder.AddColumn<int>(
                name: "UsuarioAplicacaoId",
                table: "FormularioResultados",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "FormularioResultadoValores",
                columns: table => new
                {
                    FormularioResultadoValorId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Chave = table.Column<string>(type: "text", nullable: false),
                    Valor = table.Column<string>(type: "text", nullable: true),
                    DataCadastro = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DataPreenchimento = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    FormularioResultadoId = table.Column<int>(type: "integer", nullable: false),
                    FormularioCampoId = table.Column<int>(type: "integer", nullable: false),
                    CampoId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FormularioResultadoValores", x => x.FormularioResultadoValorId);
                    table.ForeignKey(
                        name: "FK_FormularioResultadoValores_Campos_CampoId",
                        column: x => x.CampoId,
                        principalTable: "Campos",
                        principalColumn: "CampoId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_FormularioResultadoValores_FormularioCampos_FormularioCampo~",
                        column: x => x.FormularioCampoId,
                        principalTable: "FormularioCampos",
                        principalColumn: "FormularioCampoId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_FormularioResultadoValores_FormularioResultados_FormularioR~",
                        column: x => x.FormularioResultadoId,
                        principalTable: "FormularioResultados",
                        principalColumn: "FormularioResultadoId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_FormularioResultados_UsuarioAplicacaoId",
                table: "FormularioResultados",
                column: "UsuarioAplicacaoId");

            migrationBuilder.CreateIndex(
                name: "IX_FormularioResultadoValores_CampoId",
                table: "FormularioResultadoValores",
                column: "CampoId");

            migrationBuilder.CreateIndex(
                name: "IX_FormularioResultadoValores_FormularioCampoId",
                table: "FormularioResultadoValores",
                column: "FormularioCampoId");

            migrationBuilder.CreateIndex(
                name: "IX_FormularioResultadoValores_FormularioResultadoId",
                table: "FormularioResultadoValores",
                column: "FormularioResultadoId");

            migrationBuilder.AddForeignKey(
                name: "FK_FormularioResultados_Usuarios_UsuarioAplicacaoId",
                table: "FormularioResultados",
                column: "UsuarioAplicacaoId",
                principalTable: "Usuarios",
                principalColumn: "UsuarioId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FormularioResultados_Usuarios_UsuarioAplicacaoId",
                table: "FormularioResultados");

            migrationBuilder.DropTable(
                name: "FormularioResultadoValores");

            migrationBuilder.DropIndex(
                name: "IX_FormularioResultados_UsuarioAplicacaoId",
                table: "FormularioResultados");

            migrationBuilder.DropColumn(
                name: "UsuarioAplicacaoId",
                table: "FormularioResultados");

            migrationBuilder.CreateTable(
                name: "FormularioCampoValores",
                columns: table => new
                {
                    FormularioCampoValorId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    FormularioCampoId = table.Column<int>(type: "integer", nullable: false),
                    FormularioId = table.Column<int>(type: "integer", nullable: false),
                    CampoId = table.Column<int>(type: "integer", nullable: true),
                    Chave = table.Column<string>(type: "text", nullable: false),
                    DataCadastro = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DataPreenchimento = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Valor = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FormularioCampoValores", x => x.FormularioCampoValorId);
                    table.ForeignKey(
                        name: "FK_FormularioCampoValores_Campos_CampoId",
                        column: x => x.CampoId,
                        principalTable: "Campos",
                        principalColumn: "CampoId");
                    table.ForeignKey(
                        name: "FK_FormularioCampoValores_FormularioCampos_FormularioCampoId",
                        column: x => x.FormularioCampoId,
                        principalTable: "FormularioCampos",
                        principalColumn: "FormularioCampoId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_FormularioCampoValores_Formularios_FormularioId",
                        column: x => x.FormularioId,
                        principalTable: "Formularios",
                        principalColumn: "FormularioId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_FormularioCampoValores_CampoId",
                table: "FormularioCampoValores",
                column: "CampoId");

            migrationBuilder.CreateIndex(
                name: "IX_FormularioCampoValores_FormularioCampoId",
                table: "FormularioCampoValores",
                column: "FormularioCampoId");

            migrationBuilder.CreateIndex(
                name: "IX_FormularioCampoValores_FormularioId",
                table: "FormularioCampoValores",
                column: "FormularioId");
        }
    }
}
