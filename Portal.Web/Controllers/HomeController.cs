using GestaoSaudeIdosos.Application.Interfaces;
using GestaoSaudeIdosos.Web.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GestaoSaudeIdosos.Web.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly IUsuarioAppService _usuarioAppService;
        private readonly IPacienteAppService _pacienteAppService;
        private readonly IFormularioAppService _formularioAppService;

        public HomeController(
            IUsuarioAppService usuarioAppService,
            IPacienteAppService pacienteAppService,
            IFormularioAppService formularioAppService)
        {
            _usuarioAppService = usuarioAppService;
            _pacienteAppService = pacienteAppService;
            _formularioAppService = formularioAppService;
        }

        public async Task<IActionResult> Index()
        {
            var usuarios = await _usuarioAppService.AsQueryable().ToListAsync();
            var pacientes = await _pacienteAppService.AsQueryable().ToListAsync();
            var formularios = await _formularioAppService.AsQueryable().ToListAsync();

            var usuariosRecentes = usuarios
                .OrderByDescending(u => u.DataCadastro)
                .Take(5)
                .Select(u => new UsuarioListItemViewModel
                {
                    UsuarioId = u.UsuarioId,
                    Nome = u.Nome,
                    Email = u.Email,
                    Perfil = u.Perfil,
                    Ativo = u.Ativo,
                    DataCadastro = u.DataCadastro
                })
                .ToList();

            var pacientesRecentes = pacientes
                .OrderByDescending(p => p.DataCadastro)
                .Take(5)
                .Select(p => new PacienteListItemViewModel
                {
                    PacienteId = p.PacienteId,
                    Nome = p.Nome,
                    DataNascimento = p.DataNascimento,
                    Idade = p.Idade,
                    Responsavel = p.Responsavel?.Nome,
                    DataCadastro = p.DataCadastro
                })
                .ToList();

            var formulariosRecentes = formularios
                .OrderByDescending(f => f.DataCadastro)
                .Take(5)
                .Select(f => new FormularioResumoViewModel
                {
                    FormularioId = f.FormularioId,
                    Descricao = f.Descricao,
                    CamposTotais = f.Campos?.Count ?? 0,
                    PacientesAssociados = f.Pacientes?.Count ?? 0,
                    DataCadastro = f.DataCadastro
                })
                .ToList();

            var relatorios = pacientesRecentes
                .Select(MontarRelatorioPaciente)
                .ToList();

            var alertasAtivos = relatorios.Count(r => !string.Equals(r.NivelRisco, "Baixo", StringComparison.OrdinalIgnoreCase));

            var model = new DashboardViewModel
            {
                UsuarioNome = User.Identity?.Name ?? "Usuário",
                TotalUsuarios = usuarios.Count,
                TotalPacientes = pacientes.Count,
                FormulariosAtivos = formularios.Count(f => f.Ativo),
                AlertasAtivos = alertasAtivos,
                UsuariosRecentes = usuariosRecentes,
                PacientesRecentes = pacientesRecentes,
                FormulariosRecentes = formulariosRecentes,
                Relatorios = relatorios
            };

            return View(model);
        }

        private static RelatorioPacienteViewModel MontarRelatorioPaciente(PacienteListItemViewModel paciente)
        {
            var risco = "Baixo";
            var sugestao = "Manter rotina de acompanhamento atual.";

            if (paciente.Idade >= 85)
            {
                risco = "Alto";
                sugestao = "Agendar avaliação neurológica e reforçar exercícios de estimulação cognitiva.";
            }
            else if (paciente.Idade >= 75)
            {
                risco = "Moderado";
                sugestao = "Reforçar atividades físicas leves e monitorar sinais de déficit cognitivo.";
            }

            return new RelatorioPacienteViewModel
            {
                PacienteNome = paciente.Nome,
                NivelRisco = risco,
                Resumo = $"Paciente com {paciente.Idade} anos. Última atualização em {paciente.DataCadastro:dd/MM/yyyy}.",
                Sugestao = sugestao,
                UltimaAtualizacao = paciente.DataCadastro
            };
        }
    }
}
