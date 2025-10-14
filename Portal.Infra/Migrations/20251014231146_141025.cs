using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GestaoSaudeIdosos.Infra.Migrations
{
    /// <inheritdoc />
    public partial class _141025 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Campo_Usuarios_UsuarioId",
                table: "Campo");

            migrationBuilder.DropForeignKey(
                name: "FK_FormularioCampos_Campo_CampoId",
                table: "FormularioCampos");

            migrationBuilder.DropForeignKey(
                name: "FK_FormularioCampoValores_Campo_CampoId",
                table: "FormularioCampoValores");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Campo",
                table: "Campo");

            migrationBuilder.RenameTable(
                name: "Campo",
                newName: "Campos");

            migrationBuilder.RenameIndex(
                name: "IX_Campo_UsuarioId",
                table: "Campos",
                newName: "IX_Campos_UsuarioId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Campos",
                table: "Campos",
                column: "CampoId");

            migrationBuilder.AddForeignKey(
                name: "FK_Campos_Usuarios_UsuarioId",
                table: "Campos",
                column: "UsuarioId",
                principalTable: "Usuarios",
                principalColumn: "UsuarioId");

            migrationBuilder.AddForeignKey(
                name: "FK_FormularioCampos_Campos_CampoId",
                table: "FormularioCampos",
                column: "CampoId",
                principalTable: "Campos",
                principalColumn: "CampoId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_FormularioCampoValores_Campos_CampoId",
                table: "FormularioCampoValores",
                column: "CampoId",
                principalTable: "Campos",
                principalColumn: "CampoId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Campos_Usuarios_UsuarioId",
                table: "Campos");

            migrationBuilder.DropForeignKey(
                name: "FK_FormularioCampos_Campos_CampoId",
                table: "FormularioCampos");

            migrationBuilder.DropForeignKey(
                name: "FK_FormularioCampoValores_Campos_CampoId",
                table: "FormularioCampoValores");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Campos",
                table: "Campos");

            migrationBuilder.RenameTable(
                name: "Campos",
                newName: "Campo");

            migrationBuilder.RenameIndex(
                name: "IX_Campos_UsuarioId",
                table: "Campo",
                newName: "IX_Campo_UsuarioId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Campo",
                table: "Campo",
                column: "CampoId");

            migrationBuilder.AddForeignKey(
                name: "FK_Campo_Usuarios_UsuarioId",
                table: "Campo",
                column: "UsuarioId",
                principalTable: "Usuarios",
                principalColumn: "UsuarioId");

            migrationBuilder.AddForeignKey(
                name: "FK_FormularioCampos_Campo_CampoId",
                table: "FormularioCampos",
                column: "CampoId",
                principalTable: "Campo",
                principalColumn: "CampoId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_FormularioCampoValores_Campo_CampoId",
                table: "FormularioCampoValores",
                column: "CampoId",
                principalTable: "Campo",
                principalColumn: "CampoId");
        }
    }
}
