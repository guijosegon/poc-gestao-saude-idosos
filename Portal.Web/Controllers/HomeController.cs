using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using GestaoSaudeIdosos.Application.Interfaces;
using GestaoSaudeIdosos.Domain.Common.Helpers;
using GestaoSaudeIdosos.Domain.Entities;
using GestaoSaudeIdosos.Web.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GestaoSaudeIdosos.Web.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private static readonly JsonSerializerOptions GraficoJsonOptions = new()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            Converters = { new JsonStringEnumConverter() }
        };

        private readonly IUsuarioAppService _usuarioAppService;
        private readonly IPacienteAppService _pacienteAppService;
        private readonly IFormularioAppService _formularioAppService;
        private readonly IGraficoAppService _graficoAppService;

        public HomeController(
            IUsuarioAppService usuarioAppService,
            IPacienteAppService pacienteAppService,
            IFormularioAppService formularioAppService,
            IGraficoAppService graficoAppService)
        {
            _usuarioAppService = usuarioAppService;
            _pacienteAppService = pacienteAppService;
            _formularioAppService = formularioAppService;
            _graficoAppService = graficoAppService;
        }

        public async Task<IActionResult> Index()
        {
            var model = await CriarDashboardAsync();
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> PortalResumo()
        {
            var model = await CriarDashboardAsync();
            return PartialView("_PortalResumo", model);
        }

        private async Task<DashboardViewModel> CriarDashboardAsync()
        {
            var usuarios = await _usuarioAppService.AsQueryable().ToListAsync();
            var pacientes = await _pacienteAppService.AsQueryable().ToListAsync();
            var formularios = await _formularioAppService.AsQueryable().ToListAsync();
            var graficos = await _graficoAppService.AsQueryable().ToListAsync();

            var usuarioAtualEmail = User.Identity?.Name;
            var usuarioAtual = !string.IsNullOrWhiteSpace(usuarioAtualEmail)
                ? usuarios.FirstOrDefault(u => string.Equals(u.Email, usuarioAtualEmail, StringComparison.OrdinalIgnoreCase))
                : null;

            var usuariosRecentes = usuarios
                .OrderByDescending(u => u.DataCadastro)
                .Take(5)
                .Select(u => new UsuarioListItemViewModel
                {
                    UsuarioId = u.UsuarioId,
                    NomeCompleto = u.NomeCompleto,
                    Email = u.Email,
                    CpfRg = u.CpfRg,
                    ImagemPerfil = u.ImagemPerfil,
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
                    NomeCompleto = p.NomeCompleto,
                    DataNascimento = p.DataNascimento,
                    Idade = p.Idade,
                    Responsavel = p.Responsavel?.NomeCompleto,
                    ImagemPerfil = p.ImagemPerfil,
                    DataCadastro = p.DataCadastro,
                    CondicoesCronicas = p.CondicoesCronicas,
                    HistoricoCirurgico = p.HistoricoCirurgico,
                    RiscoQueda = p.RiscoQueda,
                    Mobilidade = p.Mobilidade,
                    DietasRestricoes = p.DietasRestricoes,
                    Sexo = p.Sexo
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

            var graficosPortal = graficos
                .Where(g => g.ExibirNoPortal)
                .Select(CriarGraficoPortal)
                .ToList();

            return new DashboardViewModel
            {
                UsuarioNome = !string.IsNullOrWhiteSpace(usuarioAtual?.NomeCompleto)
                    ? usuarioAtual.NomeCompleto
                    : (usuarioAtualEmail ?? "Usuário"),
                UsuarioImagemPerfil = usuarioAtual?.ImagemPerfil,
                TotalUsuarios = usuarios.Count,
                TotalPacientes = pacientes.Count,
                FormulariosAtivos = formularios.Count(f => f.Ativo),
                AlertasAtivos = alertasAtivos,
                UsuariosRecentes = usuariosRecentes,
                PacientesRecentes = pacientesRecentes,
                FormulariosRecentes = formulariosRecentes,
                Relatorios = relatorios,
                Graficos = graficosPortal
            };
        }

        private static RelatorioPacienteViewModel MontarRelatorioPaciente(PacienteListItemViewModel paciente)
        {
            const int riscoBaixo = 0;
            const int riscoModerado = 1;
            const int riscoAlto = 2;

            var nivelRisco = riscoBaixo;
            var sugestoes = new List<string>();
            var resumoPartes = new List<string>
            {
                $"Paciente com {paciente.Idade} anos."
            };

            if (!string.IsNullOrWhiteSpace(paciente.CondicoesCronicas))
            {
                resumoPartes.Add($"Condições crônicas: {paciente.CondicoesCronicas}.");

                if (ContemTermo(paciente.CondicoesCronicas, "insufici", "cardi", "avc", "neoplas", "demenc"))
                {
                    nivelRisco = Math.Max(nivelRisco, riscoAlto);
                    sugestoes.Add("Agendar avaliação especializada para manejo das condições crônicas relatadas.");
                }
                else if (ContemTermo(paciente.CondicoesCronicas, "hipertens", "diabet", "asma", "doença pulmonar", "osteopor"))
                {
                    nivelRisco = Math.Max(nivelRisco, riscoModerado);
                    sugestoes.Add("Reforçar controle clínico e acompanhamento multiprofissional das condições crônicas.");
                }
            }

            resumoPartes.Add($"Risco de quedas: {paciente.RiscoQuedaDescricao}.");

            switch (paciente.RiscoQueda)
            {
                case Enums.RiscoQuedaPaciente.Alto:
                case Enums.RiscoQuedaPaciente.QuedasRecorrentes:
                    nivelRisco = Math.Max(nivelRisco, riscoAlto);
                    sugestoes.Add("Intensificar fisioterapia e revisar plano de prevenção de quedas.");
                    break;
                case Enums.RiscoQuedaPaciente.Moderado:
                    nivelRisco = Math.Max(nivelRisco, riscoModerado);
                    sugestoes.Add("Reforçar orientações ambientais e exercícios de equilíbrio.");
                    break;
            }

            resumoPartes.Add($"Mobilidade e auxílios: {paciente.MobilidadeDescricao}.");

            if (paciente.Mobilidade is Enums.MobilidadePaciente.CadeiraDeRodas
                or Enums.MobilidadePaciente.Acamado
                or Enums.MobilidadePaciente.Andador
                or Enums.MobilidadePaciente.Bengala
                or Enums.MobilidadePaciente.ProteseOuOrtese)
            {
                nivelRisco = Math.Max(nivelRisco, riscoModerado);
                sugestoes.Add("Verificar manutenção e adequação dos dispositivos de mobilidade utilizados.");
            }
            else if (paciente.Mobilidade == Enums.MobilidadePaciente.Outro)
            {
                sugestoes.Add("Verificar manutenção e adequação dos dispositivos de mobilidade utilizados.");
            }

            if (!string.IsNullOrWhiteSpace(paciente.DietasRestricoes))
            {
                resumoPartes.Add($"Plano alimentar: {paciente.DietasRestricoes}.");
                nivelRisco = Math.Max(nivelRisco, riscoModerado);
                sugestoes.Add("Revisar plano alimentar com nutricionista para garantir adesão às restrições.");
            }

            if (!string.IsNullOrWhiteSpace(paciente.HistoricoCirurgico))
            {
                resumoPartes.Add($"Histórico cirúrgico: {paciente.HistoricoCirurgico}.");
                nivelRisco = Math.Max(nivelRisco, riscoModerado);
                sugestoes.Add("Monitorar sinais de complicações relacionadas ao histórico cirúrgico informado.");
            }

            resumoPartes.Add($"Última atualização em {paciente.DataCadastro:dd/MM/yyyy}.");

            if (!sugestoes.Any())
            {
                sugestoes.Add("Manter rotina de acompanhamento atual.");
            }

            var risco = nivelRisco switch
            {
                riscoAlto => "Alto",
                riscoModerado => "Moderado",
                _ => "Baixo"
            };

            return new RelatorioPacienteViewModel
            {
                PacienteNome = paciente.NomeCompleto,
                NivelRisco = risco,
                Resumo = string.Join(" ", resumoPartes),
                Sugestao = string.Join(" ", sugestoes.Distinct()),
                UltimaAtualizacao = paciente.DataCadastro
            };

            static bool ContemTermo(string? texto, params string[] termos)
            {
                if (string.IsNullOrWhiteSpace(texto))
                    return false;

                foreach (var termo in termos)
                {
                    if (texto.IndexOf(termo, StringComparison.OrdinalIgnoreCase) >= 0)
                        return true;
                }

                return false;
            }
        }

        private static GraficoVisualizacaoViewModel CriarGraficoPortal(Grafico grafico)
        {
            var configuracao = DeserializarConfiguracao(grafico.Configuracao);
            return new GraficoVisualizacaoViewModel
            {
                GraficoId = grafico.GraficoId,
                Descricao = grafico.Descricao,
                Origem = grafico.Origem,
                Tipo = grafico.Tipo,
                Configuracao = configuracao
            };
        }

        private static GraficoConfiguracaoModel DeserializarConfiguracao(string? configuracao)
        {
            if (string.IsNullOrWhiteSpace(configuracao))
                return new GraficoConfiguracaoModel();

            try
            {
                return JsonSerializer.Deserialize<GraficoConfiguracaoModel>(configuracao, GraficoJsonOptions)
                    ?? new GraficoConfiguracaoModel();
            }
            catch
            {
                return new GraficoConfiguracaoModel();
            }
        }
    }
}
