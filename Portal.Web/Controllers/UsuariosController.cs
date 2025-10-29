using GestaoSaudeIdosos.Application.Interfaces;
using GestaoSaudeIdosos.Domain.Common.Helpers;
using GestaoSaudeIdosos.Web.Mappers;
using GestaoSaudeIdosos.Web.Services;
using GestaoSaudeIdosos.Web.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace GestaoSaudeIdosos.Web.Controllers
{
    [Authorize]
    public class UsuariosController : Controller
    {
        private readonly IUsuarioAppService _usuarioAppService;
        private readonly IImagemStorageService _imagemStorageService;

        public UsuariosController(IUsuarioAppService usuarioAppService, IImagemStorageService imagemStorageService)
        {
            _usuarioAppService = usuarioAppService;
            _imagemStorageService = imagemStorageService;
        }

        public async Task<IActionResult> Index([FromQuery] UsuarioFiltroViewModel filtro)
        {
            filtro ??= new UsuarioFiltroViewModel();

            var query = _usuarioAppService.AsQueryable();

            if (!string.IsNullOrWhiteSpace(filtro.Busca))
                query = query.Where(u => EF.Functions.ILike(u.NomeCompleto, $"%{filtro.Busca.Trim()}%") || EF.Functions.ILike(u.Email, $"%{filtro.Busca.Trim()}%"));
            if (filtro.Perfil.HasValue)
                query = query.Where(u => u.Perfil == filtro.Perfil.Value);
            if (filtro.Ativo.HasValue)
                query = query.Where(u => u.Ativo == filtro.Ativo.Value);

            var itensPorPagina = filtro.ItensPorPagina;
            var totalRegistros = await query.CountAsync();
            var totalPaginas = totalRegistros is 0 ? 0 : (int)Math.Ceiling(totalRegistros / (double)itensPorPagina);

            var paginaAtual = filtro.Pagina;
            if (totalPaginas > 0 && paginaAtual > totalPaginas)
                paginaAtual = totalPaginas;

            var registros = await query
                .OrderByDescending(u => u.DataCadastro)
                .Skip((paginaAtual - 1) * itensPorPagina)
                .Take(itensPorPagina)
                .Select(UsuarioViewModelMapper.ToListItem)
                .ToListAsync();

            filtro.Pagina = paginaAtual;

            var model = new UsuariosIndexViewModel
            {
                Filtro = filtro,
                Paginacao = new PaginacaoViewModel
                {
                    PaginaAtual = paginaAtual,
                    TotalPaginas = totalPaginas,
                    TotalRegistros = totalRegistros,
                    ItensPorPagina = itensPorPagina
                },
                Registros = registros,
                Perfis = ObterPerfis()
            };

            return View(model);
        }

        public async Task<IActionResult> Details(int id)
        {
            var detalhe = await _usuarioAppService
                .AsQueryable()
                .Where(u => u.UsuarioId == id)
                .Select(UsuarioViewModelMapper.ToDetail)
                .FirstOrDefaultAsync();

            if (detalhe is null)
                return NotFound();

            return View(detalhe);
        }

        [Authorize(Roles = nameof(Enums.PerfilUsuario.Administrador))]
        public IActionResult Create()
        {
            ViewBag.Perfis = ObterPerfis();
            return View(new UsuarioFormViewModel());
        }

        [HttpPost]
        [Authorize(Roles = nameof(Enums.PerfilUsuario.Administrador))]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(UsuarioFormViewModel model)
        {
            ViewBag.Perfis = ObterPerfis();

            if (!ModelState.IsValid)
                return View(model);

            var existente = _usuarioAppService.AsQueryable().FirstOrDefault(f => f.Email.Equals(model.Email ?? string.Empty));

            if (existente is not null)
            {
                ModelState.AddModelError(nameof(model.Email), "Este e-mail já está cadastrado.");
                return View(model);
            }

            model.ImagemPerfil = await _imagemStorageService.SalvarAsync(model.ImagemPerfilArquivo, "usuarios");

            var usuario = model.ToEntity();

            try
            {
                await _usuarioAppService.CreateAsync(usuario);
            }
            catch (Exception ex)
            {
                _imagemStorageService.Remover(model.ImagemPerfil);

                ViewData["Erro"] = string.IsNullOrWhiteSpace(ex.Message) ? "Não foi possível criar o usuário. Tente novamente." : ex.Message;
                return View(model);
            }

            TempData["Sucesso"] = "Usuário criado com sucesso.";
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int id)
        {
            var model = await _usuarioAppService
                .AsQueryable()
                .Where(u => u.UsuarioId == id)
                .Select(UsuarioViewModelMapper.ToEditForm)
                .FirstOrDefaultAsync();

            if (model is null)
                return NotFound();

            model.PerfisDisponiveis = ObterPerfis();

            var isAdmin = User.IsInRole(Enums.PerfilUsuario.Administrador.ToString());
            var isSelf = string.Equals(User.Identity?.Name, model.Email, StringComparison.OrdinalIgnoreCase);

            if (!isAdmin && !isSelf)
                return Forbid();

            model.PermiteAlterarPerfil = isAdmin;

            if (!isAdmin)
            {
                model.PerfisDisponiveis = model.PerfisDisponiveis
                    .Where(p => string.Equals(p.Value, model.Perfil, StringComparison.OrdinalIgnoreCase))
                    .ToList();
            }

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, UsuarioEdicaoViewModel model)
        {
            var isAdmin = User.IsInRole(Enums.PerfilUsuario.Administrador.ToString());
            var isSelf = string.Equals(User.Identity?.Name, model.Email, StringComparison.OrdinalIgnoreCase);

            model.PerfisDisponiveis = ObterPerfis();
            model.PermiteAlterarPerfil = isAdmin;

            if (!isAdmin)
            {
                model.PerfisDisponiveis = model.PerfisDisponiveis
                    .Where(p => string.Equals(p.Value, model.Perfil, StringComparison.OrdinalIgnoreCase))
                    .ToList();
            }

            if (model.UsuarioId != id)
                ModelState.AddModelError(string.Empty, "Não foi possível localizar o usuário informado.");

            if (!isAdmin && !isSelf)
                return Forbid();

            if (!ModelState.IsValid)
                return View(model);

            var usuario = await _usuarioAppService.AsTracking().FirstOrDefaultAsync(u => u.UsuarioId == id);

            if (usuario is null)
                return NotFound();

            var novoEmail = model.Email.Trim();

            if (!string.Equals(usuario.Email, novoEmail, StringComparison.OrdinalIgnoreCase))
            {
                var existente = _usuarioAppService.AsQueryable().FirstOrDefault(f => f.Email == novoEmail);

                if (existente is not null && existente.UsuarioId != usuario.UsuarioId)
                {
                    ModelState.AddModelError(nameof(model.Email), "Este e-mail já está cadastrado.");
                    return View(model);
                }
            }

            if (!string.IsNullOrWhiteSpace(model.NovaSenha))
            {
                if (!isAdmin && !_usuarioAppService.VerifyPassword(usuario, model.SenhaAtual ?? string.Empty))
                {
                    ModelState.AddModelError(nameof(model.SenhaAtual), "A senha atual informada não confere.");
                    return View(model);
                }

                usuario.Senha = model.NovaSenha;
            }

            if (model.NovaImagemPerfil is { Length: > 0 })
            {
                model.RemoverImagemPerfil = false;
            }

            if (model.RemoverImagemPerfil)
            {
                _imagemStorageService.Remover(usuario.ImagemPerfil);
                model.ImagemPerfil = null;
            }
            else
            {
                var imagemAtualizada = await _imagemStorageService.SalvarAsync(model.NovaImagemPerfil, "usuarios", usuario.ImagemPerfil);
                model.ImagemPerfil = imagemAtualizada;
            }

            model.ApplyToEntity(usuario, isAdmin && model.PermiteAlterarPerfil);

            try
            {
                _usuarioAppService.Update(usuario);
            }
            catch (Exception ex)
            {
                ViewData["Erro"] = string.IsNullOrWhiteSpace(ex.Message) ? "Não foi possível atualizar o usuário. Tente novamente." : ex.Message;
                return View(model);
            }

            TempData["Sucesso"] = "Alterações salvas com sucesso.";
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int id)
        {
            var usuario = await _usuarioAppService.AsQueryable().FirstOrDefaultAsync(u => u.UsuarioId == id);

            if (usuario is null)
                return NotFound();

            var isAdmin = User.IsInRole(Enums.PerfilUsuario.Administrador.ToString());
            var isSelf = string.Equals(User.Identity?.Name, usuario.Email, StringComparison.OrdinalIgnoreCase);

            if (!isAdmin && !isSelf)
                return Forbid();

            var model = await _usuarioAppService
                .AsQueryable()
                .Where(u => u.UsuarioId == id)
                .Select(UsuarioViewModelMapper.ToDetail)
                .FirstOrDefaultAsync();

            return View(model);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ConfirmDelete(int id, string email)
        {
            var isAdmin = User.IsInRole(Enums.PerfilUsuario.Administrador.ToString());
            var isSelf = string.Equals(User.Identity?.Name, email, StringComparison.OrdinalIgnoreCase);

            if (!isAdmin && !isSelf)
                return Forbid();

            var usuario = await _usuarioAppService.AsTracking().FirstOrDefaultAsync(u => u.UsuarioId == id);

            if (usuario is null)
                return NotFound();

            _imagemStorageService.Remover(usuario.ImagemPerfil);

            _usuarioAppService.Delete(usuario);

            TempData["Sucesso"] = "Usuário removido com sucesso.";
            return RedirectToAction(nameof(Index));
        }

        private static IEnumerable<SelectListItem> ObterPerfis()
        {
            return Enum.GetValues(typeof(Enums.PerfilUsuario))
                .Cast<Enums.PerfilUsuario>()
                .Select(perfil => new SelectListItem
                {
                    Text = perfil.ToString(),
                    Value = perfil.ToString()
                });
        }
    }
}