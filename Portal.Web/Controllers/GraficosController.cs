using GestaoSaudeIdosos.Application.Interfaces;
using GestaoSaudeIdosos.Domain.Common.Helpers;
using GestaoSaudeIdosos.Domain.Entities;
using GestaoSaudeIdosos.Web.Extensions;
using GestaoSaudeIdosos.Web.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace GestaoSaudeIdosos.Web.Controllers
{
    [Authorize]
    public class GraficosController : Controller
    {
        private static readonly JsonSerializerOptions JsonOptions = new()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            Converters = { new JsonStringEnumConverter() }
        };

        private readonly IGraficoAppService _graficoAppService;
        private readonly IPacienteAppService _pacienteAppService;
        private readonly IFormularioAppService _formularioAppService;
        private readonly IFormularioResultadoAppService _formularioResultadoAppService;

        public GraficosController(
            IGraficoAppService graficoAppService,
            IPacienteAppService pacienteAppService,
            IFormularioAppService formularioAppService,
            IFormularioResultadoAppService formularioResultadoAppService)
        {
            _graficoAppService = graficoAppService;
            _pacienteAppService = pacienteAppService;
            _formularioAppService = formularioAppService;
            _formularioResultadoAppService = formularioResultadoAppService;
        }

        public async Task<IActionResult> Index()
        {
            var graficos = await _graficoAppService
                .AsQueryable()
                .AsNoTracking()
                .OrderBy(g => g.Descricao)
                .ToListAsync();

            var model = graficos
                .Select(g =>
                {
                    var visualizacao = CriarVisualizacao(g);
                    return new GraficoListItemViewModel
                    {
                        GraficoId = visualizacao.GraficoId,
                        Descricao = visualizacao.Descricao,
                        Origem = visualizacao.Origem,
                        Tipo = visualizacao.Tipo,
                        Configuracao = visualizacao.Configuracao
                    };
                })
                .ToList();

            return View(model);
        }

        public async Task<IActionResult> Details(int id)
        {
            var grafico = await _graficoAppService
                .AsQueryable()
                .AsNoTracking()
                .FirstOrDefaultAsync(f => f.GraficoId == id);

            if (grafico is null)
                return NotFound();

            var visualizacao = CriarVisualizacao(grafico);
            string? campoDescricao = null;
            string? formularioDescricao = null;

            if (grafico.Origem == Enums.TipoOrigemGrafico.Paciente)
            {
                campoDescricao = visualizacao.Configuracao.Paciente?.CampoCategoria.GetDisplayName();
            }
            else if (grafico.Origem == Enums.TipoOrigemGrafico.Formulario && visualizacao.Configuracao.Formulario is { } formularioConfig)
            {
                formularioDescricao = await ObterDescricaoFormularioAsync(formularioConfig.FormularioId);
                campoDescricao = await ObterDescricaoCampoFormularioAsync(formularioConfig.FormularioId, formularioConfig.CampoId);
            }

            var model = new GraficoDetalheViewModel
            {
                GraficoId = visualizacao.GraficoId,
                Descricao = visualizacao.Descricao,
                Origem = visualizacao.Origem,
                Tipo = visualizacao.Tipo,
                Configuracao = visualizacao.Configuracao,
                CampoDescricao = campoDescricao,
                FormularioDescricao = formularioDescricao
            };

            return View(model);
        }

        public async Task<IActionResult> Create()
        {
            var model = await PopularListasAsync(new GraficoFormViewModel());
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(GraficoFormViewModel model)
        {
            AplicarValidacaoOrigem(ModelState, model.Origem, model.PacienteCampoCategoria, model.FormularioId, model.FormularioCampoId);

            if (!ModelState.IsValid)
            {
                model = await PopularListasAsync(model);
                return View(model);
            }

            var configuracao = ConstruirConfiguracao(model);
            var grafico = new Grafico
            {
                Descricao = model.Descricao.Trim(),
                Origem = model.Origem,
                Tipo = model.Tipo,
                Configuracao = JsonSerializer.Serialize(configuracao, JsonOptions),
                ExibirNoPortal = model.ExibirNoPortal,
                FormularioId = model.Origem == Enums.TipoOrigemGrafico.Formulario ? model.FormularioId : null
            };

            try
            {
                await _graficoAppService.CreateAsync(grafico);
            }
            catch (Exception ex)
            {
                ViewData["Erro"] = string.IsNullOrWhiteSpace(ex.Message)
                    ? "Não foi possível cadastrar o gráfico. Tente novamente."
                    : ex.Message;

                model = await PopularListasAsync(model);
                return View(model);
            }

            TempData["Sucesso"] = "Gráfico cadastrado com sucesso.";
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int id)
        {
            var grafico = await _graficoAppService
                .AsQueryable()
                .AsNoTracking()
                .FirstOrDefaultAsync(f => f.GraficoId == id);

            if (grafico is null)
                return NotFound();

            var model = await CriarFormViewModelAsync(grafico);
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, GraficoFormViewModel model)
        {
            if (model.GraficoId is null || model.GraficoId != id)
                ModelState.AddModelError(string.Empty, "Não foi possível localizar o gráfico informado.");

            AplicarValidacaoOrigem(ModelState, model.Origem, model.PacienteCampoCategoria, model.FormularioId, model.FormularioCampoId);

            if (!ModelState.IsValid)
            {
                model = await PopularListasAsync(model);
                return View(model);
            }

            var grafico = await _graficoAppService.AsQueryable().FirstOrDefaultAsync(f => f.GraficoId == id);

            if (grafico is null)
                return NotFound();

            var configuracao = ConstruirConfiguracao(model);

            grafico.Descricao = model.Descricao.Trim();
            grafico.Origem = model.Origem;
            grafico.Tipo = model.Tipo;
            grafico.Configuracao = JsonSerializer.Serialize(configuracao, JsonOptions);
            grafico.ExibirNoPortal = model.ExibirNoPortal;
            grafico.FormularioId = model.Origem == Enums.TipoOrigemGrafico.Formulario ? model.FormularioId : null;

            try
            {
                _graficoAppService.Update(grafico);
            }
            catch (Exception ex)
            {
                ViewData["Erro"] = string.IsNullOrWhiteSpace(ex.Message)
                    ? "Não foi possível atualizar o gráfico. Tente novamente."
                    : ex.Message;

                model = await PopularListasAsync(model);
                return View(model);
            }

            TempData["Sucesso"] = "Gráfico atualizado com sucesso.";
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int id)
        {
            var grafico = await _graficoAppService
                .AsQueryable()
                .AsNoTracking()
                .FirstOrDefaultAsync(f => f.GraficoId == id);

            if (grafico is null)
                return NotFound();

            var visualizacao = CriarVisualizacao(grafico);
            string? campoDescricao = null;
            string? formularioDescricao = null;

            if (grafico.Origem == Enums.TipoOrigemGrafico.Paciente)
            {
                campoDescricao = visualizacao.Configuracao.Paciente?.CampoCategoria.GetDisplayName();
            }
            else if (grafico.Origem == Enums.TipoOrigemGrafico.Formulario && visualizacao.Configuracao.Formulario is { } formularioConfig)
            {
                formularioDescricao = await ObterDescricaoFormularioAsync(formularioConfig.FormularioId);
                campoDescricao = await ObterDescricaoCampoFormularioAsync(formularioConfig.FormularioId, formularioConfig.CampoId);
            }

            var model = new GraficoDetalheViewModel
            {
                GraficoId = visualizacao.GraficoId,
                Descricao = visualizacao.Descricao,
                Origem = visualizacao.Origem,
                Tipo = visualizacao.Tipo,
                Configuracao = visualizacao.Configuracao,
                CampoDescricao = campoDescricao,
                FormularioDescricao = formularioDescricao
            };

            return View(model);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ConfirmDelete(int id)
        {
            var grafico = await _graficoAppService.AsQueryable().FirstOrDefaultAsync(f => f.GraficoId == id);

            if (grafico is null)
                return NotFound();

            _graficoAppService.Delete(grafico);

            TempData["Sucesso"] = "Gráfico removido com sucesso.";
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> CamposFormulario(int formularioId)
        {
            var campos = await ObterCamposFormularioAsync(formularioId);
            var itens = campos.Select(c => new { value = c.Value, text = c.Text });
            return Json(new { success = true, campos = itens });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Preview([FromBody] GraficoPreviewRequest request)
        {
            if (request is null)
                return BadRequest(new { success = false, message = "Solicitação inválida." });

            var erros = ValidarOrigem(request.Origem, request.PacienteCampoCategoria, request.FormularioId, request.FormularioCampoId);

            if (erros.Any())
                return BadRequest(new { success = false, errors = erros });

            var configuracao = ConstruirConfiguracao(request);
            var titulo = string.IsNullOrWhiteSpace(request.Descricao) ? "Pré-visualização" : request.Descricao.Trim();

            try
            {
                var dados = await GerarDadosGraficoAsync(request.Origem, request.Tipo, configuracao, titulo);
                return Ok(new { success = true, dados });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        [HttpGet]
        public async Task<IActionResult> Dados(int id)
        {
            var grafico = await _graficoAppService
                .AsQueryable()
                .AsNoTracking()
                .FirstOrDefaultAsync(g => g.GraficoId == id);

            if (grafico is null)
                return NotFound();

            var configuracao = DeserializarConfiguracao(grafico.Configuracao);

            try
            {
                var dados = await GerarDadosGraficoAsync(grafico.Origem, grafico.Tipo, configuracao, grafico.Descricao);
                return Ok(new { success = true, dados, grafico = new { grafico.GraficoId, grafico.Descricao, Tipo = grafico.Tipo } });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        private async Task<GraficoFormViewModel> CriarFormViewModelAsync(Grafico grafico)
        {
            var configuracao = DeserializarConfiguracao(grafico.Configuracao);

            var model = new GraficoFormViewModel
            {
                GraficoId = grafico.GraficoId,
                Descricao = grafico.Descricao,
                Origem = grafico.Origem,
                Tipo = grafico.Tipo,
                ExibirNoPortal = grafico.ExibirNoPortal,
                TituloEixoHorizontal = configuracao.TituloEixoHorizontal,
                TituloEixoVertical = configuracao.TituloEixoVertical,
                MostrarLegenda = configuracao.MostrarLegenda
            };

            if (grafico.Origem == Enums.TipoOrigemGrafico.Paciente)
            {
                model.PacienteCampoCategoria = configuracao.Paciente?.CampoCategoria;
            }
            else if (grafico.Origem == Enums.TipoOrigemGrafico.Formulario)
            {
                var formularioId = configuracao.Formulario?.FormularioId ?? grafico.FormularioId;
                model.FormularioId = formularioId;
                model.FormularioCampoId = configuracao.Formulario?.CampoId;
            }

            return await PopularListasAsync(model);
        }

        private async Task<GraficoFormViewModel> PopularListasAsync(GraficoFormViewModel model)
        {
            model.Origens = CriarSelectList(model.Origem);
            model.TiposGrafico = CriarSelectList(model.Tipo);
            model.CamposPaciente = CriarSelectList(model.PacienteCampoCategoria);
            model.Formularios = await ObterFormulariosAsync(model.FormularioId);

            if (model.FormularioId.HasValue)
            {
                model.CamposFormulario = await ObterCamposFormularioAsync(model.FormularioId.Value, model.FormularioCampoId);
            }
            else
            {
                model.CamposFormulario = Enumerable.Empty<SelectListItem>();
            }

            return model;
        }

        private static IEnumerable<SelectListItem> CriarSelectList(Enums.TipoOrigemGrafico selecionado)
        {
            return Enum.GetValues<Enums.TipoOrigemGrafico>()
                .Select(valor => new SelectListItem
                {
                    Value = ((int)valor).ToString(),
                    Text = valor.GetDisplayName(),
                    Selected = valor == selecionado
                })
                .ToList();
        }

        private static IEnumerable<SelectListItem> CriarSelectList(Enums.TipoGrafico selecionado)
        {
            return Enum.GetValues<Enums.TipoGrafico>()
                .Select(valor => new SelectListItem
                {
                    Value = ((int)valor).ToString(),
                    Text = valor.GetDisplayName(),
                    Selected = valor == selecionado
                })
                .ToList();
        }

        private static IEnumerable<SelectListItem> CriarSelectList(Enums.GraficoPacienteCampo? selecionado)
        {
            return Enum.GetValues<Enums.GraficoPacienteCampo>()
                .Select(valor => new SelectListItem
                {
                    Value = ((int)valor).ToString(),
                    Text = valor.GetDisplayName(),
                    Selected = selecionado.HasValue && valor == selecionado.Value
                })
                .ToList();
        }

        private async Task<IEnumerable<SelectListItem>> ObterFormulariosAsync(int? selecionadoId)
        {
            var formularios = await _formularioAppService
                .AsQueryable()
                .AsNoTracking()
                .Where(f => f.Ativo)
                .OrderBy(f => f.Descricao)
                .Select(f => new { f.FormularioId, f.Descricao })
                .ToListAsync();

            return formularios
                .Select(f => new SelectListItem
                {
                    Value = f.FormularioId.ToString(),
                    Text = f.Descricao,
                    Selected = selecionadoId.HasValue && f.FormularioId == selecionadoId.Value
                })
                .ToList();
        }

        private async Task<IEnumerable<SelectListItem>> ObterCamposFormularioAsync(int formularioId, int? selecionadoCampoId = null)
        {
            var campos = await _formularioAppService
                .AsQueryable()
                .AsNoTracking()
                .Where(f => f.FormularioId == formularioId)
                .SelectMany(f => f.Campos.OrderBy(fc => fc.Ordem))
                .Select(fc => new
                {
                    fc.CampoId,
                    Descricao = fc.Campo != null && !string.IsNullOrWhiteSpace(fc.Campo.Descricao)
                        ? fc.Campo.Descricao
                        : $"Campo {fc.CampoId}"
                })
                .ToListAsync();

            return campos
                .Select(c => new SelectListItem
                {
                    Value = c.CampoId.ToString(),
                    Text = c.Descricao,
                    Selected = selecionadoCampoId.HasValue && c.CampoId == selecionadoCampoId.Value
                })
                .ToList();
        }

        private async Task<string?> ObterDescricaoFormularioAsync(int formularioId)
        {
            var descricao = await _formularioAppService
                .AsQueryable()
                .AsNoTracking()
                .Where(f => f.FormularioId == formularioId)
                .Select(f => f.Descricao)
                .FirstOrDefaultAsync();

            return string.IsNullOrWhiteSpace(descricao) ? null : descricao;
        }

        private static GraficoConfiguracaoModel ConstruirConfiguracao(GraficoFormViewModel model)
        {
            var configuracao = new GraficoConfiguracaoModel
            {
                TituloEixoHorizontal = string.IsNullOrWhiteSpace(model.TituloEixoHorizontal) ? null : model.TituloEixoHorizontal.Trim(),
                TituloEixoVertical = string.IsNullOrWhiteSpace(model.TituloEixoVertical) ? null : model.TituloEixoVertical.Trim(),
                MostrarLegenda = model.MostrarLegenda
            };

            if (model.Origem == Enums.TipoOrigemGrafico.Paciente && model.PacienteCampoCategoria.HasValue)
            {
                configuracao.Paciente = new GraficoConfiguracaoPacienteModel
                {
                    CampoCategoria = model.PacienteCampoCategoria.Value
                };
            }
            else if (model.Origem == Enums.TipoOrigemGrafico.Formulario && model.FormularioId.HasValue && model.FormularioCampoId.HasValue)
            {
                configuracao.Formulario = new GraficoConfiguracaoFormularioModel
                {
                    FormularioId = model.FormularioId.Value,
                    CampoId = model.FormularioCampoId.Value
                };
            }

            return configuracao;
        }

        private static GraficoConfiguracaoModel ConstruirConfiguracao(GraficoPreviewRequest request)
        {
            var configuracao = new GraficoConfiguracaoModel
            {
                TituloEixoHorizontal = string.IsNullOrWhiteSpace(request.TituloEixoHorizontal) ? null : request.TituloEixoHorizontal.Trim(),
                TituloEixoVertical = string.IsNullOrWhiteSpace(request.TituloEixoVertical) ? null : request.TituloEixoVertical.Trim(),
                MostrarLegenda = request.MostrarLegenda
            };

            if (request.Origem == Enums.TipoOrigemGrafico.Paciente && request.PacienteCampoCategoria.HasValue)
            {
                configuracao.Paciente = new GraficoConfiguracaoPacienteModel
                {
                    CampoCategoria = request.PacienteCampoCategoria.Value
                };
            }
            else if (request.Origem == Enums.TipoOrigemGrafico.Formulario && request.FormularioId.HasValue && request.FormularioCampoId.HasValue)
            {
                configuracao.Formulario = new GraficoConfiguracaoFormularioModel
                {
                    FormularioId = request.FormularioId.Value,
                    CampoId = request.FormularioCampoId.Value
                };
            }

            return configuracao;
        }

        private static GraficoConfiguracaoModel DeserializarConfiguracao(string? configuracao)
        {
            if (string.IsNullOrWhiteSpace(configuracao))
                return new GraficoConfiguracaoModel();

            try
            {
                return JsonSerializer.Deserialize<GraficoConfiguracaoModel>(configuracao, JsonOptions) ?? new GraficoConfiguracaoModel();
            }
            catch
            {
                return new GraficoConfiguracaoModel();
            }
        }

        private GraficoVisualizacaoViewModel CriarVisualizacao(Grafico grafico)
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

        private static void AplicarValidacaoOrigem(ModelStateDictionary modelState, Enums.TipoOrigemGrafico origem, Enums.GraficoPacienteCampo? campoPaciente, int? formularioId, int? formularioCampoId)
        {
            if (origem == Enums.TipoOrigemGrafico.Paciente && !campoPaciente.HasValue)
            {
                modelState.AddModelError(nameof(GraficoFormViewModel.PacienteCampoCategoria), "Selecione o campo de paciente que será usado no gráfico.");
            }
            else if (origem == Enums.TipoOrigemGrafico.Formulario)
            {
                if (!formularioId.HasValue)
                    modelState.AddModelError(nameof(GraficoFormViewModel.FormularioId), "Selecione o formulário base do gráfico.");

                if (!formularioCampoId.HasValue)
                    modelState.AddModelError(nameof(GraficoFormViewModel.FormularioCampoId), "Selecione o campo do formulário utilizado para o gráfico.");
            }
        }

        private static IReadOnlyCollection<string> ValidarOrigem(Enums.TipoOrigemGrafico origem, Enums.GraficoPacienteCampo? campoPaciente, int? formularioId, int? formularioCampoId)
        {
            var erros = new List<string>();

            if (origem == Enums.TipoOrigemGrafico.Paciente)
            {
                if (!campoPaciente.HasValue)
                    erros.Add("Selecione o campo de paciente que será usado no gráfico.");
            }
            else if (origem == Enums.TipoOrigemGrafico.Formulario)
            {
                if (!formularioId.HasValue)
                    erros.Add("Selecione o formulário base do gráfico.");

                if (!formularioCampoId.HasValue)
                    erros.Add("Selecione o campo do formulário utilizado para o gráfico.");
            }

            return erros;
        }

        private async Task<object> GerarDadosGraficoAsync(Enums.TipoOrigemGrafico origem, Enums.TipoGrafico tipo, GraficoConfiguracaoModel configuracao, string titulo)
        {
            var categoriaLabel = "Categoria";
            var valorLabel = "Quantidade";
            List<GraficoSerieItem> dados;

            if (origem == Enums.TipoOrigemGrafico.Paciente)
            {
                if (configuracao.Paciente is null)
                    throw new InvalidOperationException("Configuração do gráfico de pacientes inválida.");

                dados = await ObterDadosPacienteAsync(configuracao.Paciente.CampoCategoria);
                categoriaLabel = ObterTituloCampoPaciente(configuracao.Paciente.CampoCategoria);
            }
            else if (origem == Enums.TipoOrigemGrafico.Formulario)
            {
                if (configuracao.Formulario is null)
                    throw new InvalidOperationException("Configuração do gráfico de formulários inválida.");

                dados = await ObterDadosFormularioAsync(configuracao.Formulario.FormularioId, configuracao.Formulario.CampoId);
                categoriaLabel = await ObterDescricaoCampoFormularioAsync(configuracao.Formulario.FormularioId, configuracao.Formulario.CampoId);
            }
            else
            {
                dados = new List<GraficoSerieItem>();
            }

            var linhas = dados.Select(d => new object[] { d.Categoria, d.Valor }).ToList();
            var opcoes = CriarOpcoesGrafico(tipo, titulo, configuracao, categoriaLabel, valorLabel);

            return new
            {
                columnLabels = new[] { categoriaLabel, valorLabel },
                rows = linhas,
                options = opcoes
            };
        }

        private async Task<List<GraficoSerieItem>> ObterDadosPacienteAsync(Enums.GraficoPacienteCampo campo)
        {
            var pacientes = await _pacienteAppService
                .AsQueryable()
                .AsNoTracking()
                .Select(p => new
                {
                    p.Sexo,
                    p.RiscoQueda,
                    p.Mobilidade,
                    p.Ativo,
                    p.Idade
                })
                .ToListAsync();

            return campo switch
            {
                Enums.GraficoPacienteCampo.Sexo => pacientes
                    .GroupBy(p => p.Sexo)
                    .Select(g => new GraficoSerieItem(g.Key.GetDisplayName(), g.Count()))
                    .OrderByDescending(g => g.Valor)
                    .ToList(),

                Enums.GraficoPacienteCampo.RiscoQueda => pacientes
                    .GroupBy(p => p.RiscoQueda)
                    .Select(g => new GraficoSerieItem(g.Key.GetDisplayName(), g.Count()))
                    .OrderByDescending(g => g.Valor)
                    .ToList(),

                Enums.GraficoPacienteCampo.Mobilidade => pacientes
                    .GroupBy(p => p.Mobilidade)
                    .Select(g => new GraficoSerieItem(g.Key.GetDisplayName(), g.Count()))
                    .OrderByDescending(g => g.Valor)
                    .ToList(),

                Enums.GraficoPacienteCampo.StatusAtivo => pacientes
                    .GroupBy(p => p.Ativo)
                    .Select(g => new GraficoSerieItem(ObterDescricaoStatusAtivo(g.Key), g.Count()))
                    .OrderByDescending(g => g.Valor)
                    .ToList(),

                Enums.GraficoPacienteCampo.FaixaEtaria => pacientes
                    .GroupBy(p => ObterDescricaoFaixaEtaria(p.Idade))
                    .Select(g => new GraficoSerieItem(g.Key, g.Count()))
                    .OrderByDescending(g => g.Valor)
                    .ToList(),

                _ => new List<GraficoSerieItem>()
            };
        }

        private async Task<List<GraficoSerieItem>> ObterDadosFormularioAsync(int formularioId, int campoId)
        {
            var valores = await _formularioResultadoAppService
                .AsQueryable(r => r.Valores)
                .AsNoTracking()
                .Where(r => r.FormularioId == formularioId)
                .SelectMany(r => r.Valores.Where(v => v.CampoId == campoId))
                .Select(v => v.Valor)
                .ToListAsync();

            return valores
                .Select(valor => string.IsNullOrWhiteSpace(valor) ? "Sem resposta" : valor.Trim())
                .GroupBy(valor => valor)
                .Select(g => new GraficoSerieItem(g.Key, g.Count()))
                .OrderByDescending(g => g.Valor)
                .ToList();
        }

        private async Task<string> ObterDescricaoCampoFormularioAsync(int formularioId, int campoId)
        {
            var descricao = await _formularioAppService
                .AsQueryable()
                .Where(f => f.FormularioId == formularioId)
                .SelectMany(f => f.Campos)
                .Where(fc => fc.CampoId == campoId)
                .Select(fc => fc.Campo != null && !string.IsNullOrWhiteSpace(fc.Campo.Descricao)
                    ? fc.Campo.Descricao
                    : $"Campo {fc.CampoId}")
                .FirstOrDefaultAsync();

            return string.IsNullOrWhiteSpace(descricao) ? "Resposta" : descricao;
        }

        private static Dictionary<string, object?> CriarOpcoesGrafico(Enums.TipoGrafico tipo, string titulo, GraficoConfiguracaoModel configuracao, string categoriaLabel, string valorLabel)
        {
            var eixoHorizontal = string.IsNullOrWhiteSpace(configuracao.TituloEixoHorizontal)
                ? categoriaLabel
                : configuracao.TituloEixoHorizontal;
            var eixoVertical = string.IsNullOrWhiteSpace(configuracao.TituloEixoVertical)
                ? valorLabel
                : configuracao.TituloEixoVertical;

            var opcoes = new Dictionary<string, object?>
            {
                ["title"] = titulo,
                ["legend"] = new { position = configuracao.MostrarLegenda ? "right" : "none" },
                ["chartArea"] = new { width = "80%", height = "70%" }
            };

            switch (tipo)
            {
                case Enums.TipoGrafico.Pizza:
                    opcoes["pieHole"] = 0.25;
                    break;
                case Enums.TipoGrafico.Barra:
                    opcoes["bars"] = "horizontal";
                    opcoes["hAxis"] = new { title = eixoVertical };
                    opcoes["vAxis"] = new { title = eixoHorizontal };
                    break;
                case Enums.TipoGrafico.Linha:
                    opcoes["curveType"] = "function";
                    opcoes["legend"] = new { position = configuracao.MostrarLegenda ? "bottom" : "none" };
                    opcoes["hAxis"] = new { title = eixoHorizontal };
                    opcoes["vAxis"] = new { title = eixoVertical };
                    break;
                default:
                    opcoes["hAxis"] = new { title = eixoHorizontal };
                    opcoes["vAxis"] = new { title = eixoVertical };
                    break;
            }

            return opcoes;
        }

        private static string ObterTituloCampoPaciente(Enums.GraficoPacienteCampo campo)
        {
            return campo switch
            {
                Enums.GraficoPacienteCampo.StatusAtivo => "Situação",
                Enums.GraficoPacienteCampo.FaixaEtaria => "Faixa etária",
                _ => campo.GetDisplayName()
            };
        }

        private static string ObterDescricaoStatusAtivo(bool ativo)
        {
            return ativo ? "Ativo" : "Inativo";
        }

        private static string ObterDescricaoFaixaEtaria(int idade)
        {
            if (idade <= 0)
                return "Não informado";

            if (idade < 60)
                return "Menos de 60";

            if (idade < 70)
                return "60 a 69 anos";

            if (idade < 80)
                return "70 a 79 anos";

            if (idade < 90)
                return "80 a 89 anos";

            return "90 anos ou mais";
        }

        private record GraficoSerieItem(string Categoria, double Valor);

        public class GraficoPreviewRequest
        {
            public string? Descricao { get; set; }
            public Enums.TipoOrigemGrafico Origem { get; set; }
            public Enums.TipoGrafico Tipo { get; set; }
            public Enums.GraficoPacienteCampo? PacienteCampoCategoria { get; set; }
            public int? FormularioId { get; set; }
            public int? FormularioCampoId { get; set; }
            public string? TituloEixoHorizontal { get; set; }
            public string? TituloEixoVertical { get; set; }
            public bool MostrarLegenda { get; set; } = true;
        }
    }
}
