using GestaoSaudeIdosos.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace GestaoSaudeIdosos.Infra.Contexts
{
    public class AppContext : DbContext
    {
        public AppContext(DbContextOptions<AppContext> options) : base(options)
        {
        }

        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Paciente> Pacientes { get; set; }
        public DbSet<Campo> Campos { get; set; }
        public DbSet<Grafico> Graficos { get; set; }
        public DbSet<Formulario> Formularios { get; set; }
        public DbSet<FormularioCampo> FormularioCampos { get; set; }
        public DbSet<FormularioCampoValor> FormularioCampoValores { get; set; }
        public DbSet<FormularioResultado> FormularioResultados { get; set; }
    }
}