using GestaoSaudeIdosos.Domain.Entities;
using GestaoSaudeIdosos.Domain.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;

namespace GestaoSaudeIdosos.Infra.Repositories
{
    public class FormularioResultadoRepository : Repository<FormularioResultado>, IFormularioResultadoRepository
    {
        public FormularioResultadoRepository(Contexts.AppContext dbContext) : base(dbContext)
        {
        }

        public async Task<FormularioResultado?> ObterCompletoPorIdAsync(int id)
        {
            return await _dbContext.FormularioResultados
                .AsNoTracking()
                .Include(r => r.Formulario)
                    .ThenInclude(f => f.Campos)
                        .ThenInclude(fc => fc.Campo)
                .Include(r => r.Paciente)
                .Include(r => r.UsuarioAplicacao)
                .Include(r => r.Valores)
                    .ThenInclude(v => v.Campo)
                .Include(r => r.Valores)
                    .ThenInclude(v => v.FormularioCampo)
                .FirstOrDefaultAsync(r => r.FormularioResultadoId == id);
        }

        public async Task<List<FormularioResultado>> ListarPorPacienteAsync(int pacienteId, int? limite = null)
        {
            var query = _dbContext.FormularioResultados
                .AsNoTracking()
                .Include(r => r.Formulario)
                .Include(r => r.UsuarioAplicacao)
                .Include(r => r.Valores)
                    .ThenInclude(v => v.Campo)
                .Where(r => r.PacienteId == pacienteId);

            if (limite.HasValue)
            {
                query = query.Take(limite.Value);
            }

            return await query.OrderByDescending(r => r.DataPreenchimento).ToListAsync();
        }

        public async Task<List<FormularioResultado>> ListarPorFormularioAsync(int formularioId, int? limite = null)
        {
            var query = _dbContext.FormularioResultados
                .AsNoTracking()
                .Include(r => r.Paciente)
                .Include(r => r.Formulario)
                .Include(r => r.UsuarioAplicacao)
                .Include(r => r.Valores)
                    .ThenInclude(v => v.Campo)
                .Where(r => r.FormularioId == formularioId);

            if (limite.HasValue)
            {
                query = query.Take(limite.Value);
            }

            return await query.OrderByDescending(r => r.DataPreenchimento).ToListAsync();
        }
    }
}
