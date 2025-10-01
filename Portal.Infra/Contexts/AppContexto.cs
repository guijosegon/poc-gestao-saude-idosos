using GestaoSaudeIdosos.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace GestaoSaudeIdosos.Infra.Contexts
{
    public class AppContexto : DbContext
    {
        public AppContexto(DbContextOptions<AppContexto> options) : base(options)
        {
        }

        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Paciente> Pacientes { get; set; }           
        public DbSet<Grafico> Graficos { get; set; }
        public DbSet<Formulario> Formularios { get; set; }
        public DbSet<FormularioCampo> FormularioCampos { get; set; }
        public DbSet<FormularioCampoValor> FormularioCampoValores { get; set; }
        public DbSet<FormularioResultado> FormularioResultados { get; set; }
    }
}