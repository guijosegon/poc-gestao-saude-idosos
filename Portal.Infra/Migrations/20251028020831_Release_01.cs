using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace GestaoSaudeIdosos.Infra.Migrations
{
    /// <inheritdoc />
    public partial class Release_01 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Graficos",
                columns: table => new
                {
                    GraficoId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Descricao = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Graficos", x => x.GraficoId);
                });

            migrationBuilder.CreateTable(
                name: "Usuarios",
                columns: table => new
                {
                    UsuarioId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Chave = table.Column<string>(type: "text", nullable: false),
                    NomeCompleto = table.Column<string>(type: "text", nullable: false),
                    Email = table.Column<string>(type: "text", nullable: false),
                    Senha = table.Column<string>(type: "text", nullable: false),
                    CpfRg = table.Column<string>(type: "text", nullable: true),
                    ImagemPerfil = table.Column<string>(type: "text", nullable: true),
                    Ativo = table.Column<bool>(type: "boolean", nullable: false),
                    DataCadastro = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Perfil = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Usuarios", x => x.UsuarioId);
                });

            migrationBuilder.CreateTable(
                name: "Campos",
                columns: table => new
                {
                    CampoId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Chave = table.Column<string>(type: "text", nullable: false),
                    Descricao = table.Column<string>(type: "text", nullable: false),
                    Tipo = table.Column<int>(type: "integer", nullable: false),
                    Opcoes = table.Column<List<string>>(type: "text[]", nullable: false),
                    TextoAjuda = table.Column<string>(type: "text", nullable: false),
                    Ativo = table.Column<bool>(type: "boolean", nullable: false),
                    DataCadastro = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UsuarioId = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Campos", x => x.CampoId);
                    table.ForeignKey(
                        name: "FK_Campos_Usuarios_UsuarioId",
                        column: x => x.UsuarioId,
                        principalTable: "Usuarios",
                        principalColumn: "UsuarioId");
                });

            migrationBuilder.CreateTable(
                name: "Formularios",
                columns: table => new
                {
                    FormularioId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Chave = table.Column<string>(type: "text", nullable: false),
                    Descricao = table.Column<string>(type: "text", nullable: false),
                    DataCadastro = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Ativo = table.Column<bool>(type: "boolean", nullable: false),
                    UsuarioId = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Formularios", x => x.FormularioId);
                    table.ForeignKey(
                        name: "FK_Formularios_Usuarios_UsuarioId",
                        column: x => x.UsuarioId,
                        principalTable: "Usuarios",
                        principalColumn: "UsuarioId");
                });

            migrationBuilder.CreateTable(
                name: "FormularioCampos",
                columns: table => new
                {
                    FormularioCampoId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Chave = table.Column<string>(type: "text", nullable: false),
                    Ordem = table.Column<int>(type: "integer", nullable: false),
                    Obrigatorio = table.Column<bool>(type: "boolean", nullable: false),
                    DataCadastro = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    FormularioId = table.Column<int>(type: "integer", nullable: false),
                    CampoId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FormularioCampos", x => x.FormularioCampoId);
                    table.ForeignKey(
                        name: "FK_FormularioCampos_Campos_CampoId",
                        column: x => x.CampoId,
                        principalTable: "Campos",
                        principalColumn: "CampoId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_FormularioCampos_Formularios_FormularioId",
                        column: x => x.FormularioId,
                        principalTable: "Formularios",
                        principalColumn: "FormularioId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Pacientes",
                columns: table => new
                {
                    PacienteId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Chave = table.Column<string>(type: "text", nullable: false),
                    NomeCompleto = table.Column<string>(type: "text", nullable: false),
                    CpfRg = table.Column<string>(type: "text", nullable: true),
                    ImagemPerfil = table.Column<string>(type: "text", nullable: true),
                    DietasRestricoes = table.Column<string>(type: "text", nullable: true),
                    CondicoesCronicas = table.Column<string>(type: "text", nullable: true),
                    HistoricoCirurgico = table.Column<string>(type: "text", nullable: true),
                    RiscoQueda = table.Column<int>(type: "integer", nullable: false),
                    Mobilidade = table.Column<int>(type: "integer", nullable: false),
                    DataNascimento = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Idade = table.Column<int>(type: "integer", nullable: false),
                    Ativo = table.Column<bool>(type: "boolean", nullable: false),
                    DataCadastro = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ResponsavelId = table.Column<int>(type: "integer", nullable: true),
                    UsuarioCadastroId = table.Column<int>(type: "integer", nullable: true),
                    FormularioId = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Pacientes", x => x.PacienteId);
                    table.ForeignKey(
                        name: "FK_Pacientes_Formularios_FormularioId",
                        column: x => x.FormularioId,
                        principalTable: "Formularios",
                        principalColumn: "FormularioId");
                    table.ForeignKey(
                        name: "FK_Pacientes_Usuarios_ResponsavelId",
                        column: x => x.ResponsavelId,
                        principalTable: "Usuarios",
                        principalColumn: "UsuarioId");
                    table.ForeignKey(
                        name: "FK_Pacientes_Usuarios_UsuarioCadastroId",
                        column: x => x.UsuarioCadastroId,
                        principalTable: "Usuarios",
                        principalColumn: "UsuarioId");
                });

            migrationBuilder.CreateTable(
                name: "FormularioCampoValores",
                columns: table => new
                {
                    FormularioCampoValorId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Chave = table.Column<string>(type: "text", nullable: false),
                    Valor = table.Column<string>(type: "text", nullable: false),
                    DataCadastro = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DataPreenchimento = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    FormularioCampoId = table.Column<int>(type: "integer", nullable: false),
                    FormularioId = table.Column<int>(type: "integer", nullable: false),
                    CampoId = table.Column<int>(type: "integer", nullable: true)
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

            migrationBuilder.CreateTable(
                name: "FormularioResultados",
                columns: table => new
                {
                    FormularioResultadoId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Chave = table.Column<string>(type: "text", nullable: false),
                    DataPreenchimento = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Ativo = table.Column<bool>(type: "boolean", nullable: false),
                    DataCadastro = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    PacienteId = table.Column<int>(type: "integer", nullable: false),
                    FormularioId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FormularioResultados", x => x.FormularioResultadoId);
                    table.ForeignKey(
                        name: "FK_FormularioResultados_Formularios_FormularioId",
                        column: x => x.FormularioId,
                        principalTable: "Formularios",
                        principalColumn: "FormularioId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_FormularioResultados_Pacientes_PacienteId",
                        column: x => x.PacienteId,
                        principalTable: "Pacientes",
                        principalColumn: "PacienteId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Campos_UsuarioId",
                table: "Campos",
                column: "UsuarioId");

            migrationBuilder.CreateIndex(
                name: "IX_FormularioCampos_CampoId",
                table: "FormularioCampos",
                column: "CampoId");

            migrationBuilder.CreateIndex(
                name: "IX_FormularioCampos_FormularioId",
                table: "FormularioCampos",
                column: "FormularioId");

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

            migrationBuilder.CreateIndex(
                name: "IX_FormularioResultados_FormularioId",
                table: "FormularioResultados",
                column: "FormularioId");

            migrationBuilder.CreateIndex(
                name: "IX_FormularioResultados_PacienteId",
                table: "FormularioResultados",
                column: "PacienteId");

            migrationBuilder.CreateIndex(
                name: "IX_Formularios_UsuarioId",
                table: "Formularios",
                column: "UsuarioId");

            migrationBuilder.CreateIndex(
                name: "IX_Pacientes_FormularioId",
                table: "Pacientes",
                column: "FormularioId");

            migrationBuilder.CreateIndex(
                name: "IX_Pacientes_ResponsavelId",
                table: "Pacientes",
                column: "ResponsavelId");

            migrationBuilder.CreateIndex(
                name: "IX_Pacientes_UsuarioCadastroId",
                table: "Pacientes",
                column: "UsuarioCadastroId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FormularioCampoValores");

            migrationBuilder.DropTable(
                name: "FormularioResultados");

            migrationBuilder.DropTable(
                name: "Graficos");

            migrationBuilder.DropTable(
                name: "FormularioCampos");

            migrationBuilder.DropTable(
                name: "Pacientes");

            migrationBuilder.DropTable(
                name: "Campos");

            migrationBuilder.DropTable(
                name: "Formularios");

            migrationBuilder.DropTable(
                name: "Usuarios");
        }
    }
}
