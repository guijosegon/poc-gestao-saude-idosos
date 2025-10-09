using GestaoSaudeIdosos.API.DTOs;
using GestaoSaudeIdosos.API.Mappers;
using GestaoSaudeIdosos.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GestaoSaudeIdosos.API.Controllers
{
    [ApiController]
    [Route("api/usuarios")]
    public class UsuarioController : ControllerBase
    {
        private readonly IUsuarioAppService _usuarioAppService;

        public UsuarioController(IUsuarioAppService usuarioAppService)
        {
            _usuarioAppService = usuarioAppService;
        }

        [HttpGet]
        [Authorize]
        public async Task<ActionResult<IEnumerable<UsuarioDto>>> GetAll()
        {
            var usuarios = await _usuarioAppService.AsQueryable().ToListAsync();

            var dtos = usuarios.Select(u => u.ToDto()).ToList();
            
            if (!dtos.Any()) return NotFound();

            return Ok(dtos);
        }

        [HttpGet("{id:int}")]
        [Authorize]
        public async Task<ActionResult<UsuarioDto>> GetById(int id)
        {
            var usuario = await _usuarioAppService.AsQueryable().FirstOrDefaultAsync(u => u.UsuarioId == id);

            if (usuario is null) return NotFound();

            return Ok(usuario.ToDto());
        }

        [HttpPost]
        [Authorize(Roles = "Administrador")]
        public async Task<ActionResult<UsuarioDto>> Create([FromBody] UsuarioDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var existente = _usuarioAppService.AsQueryable().FirstOrDefault(f => f.Email.Equals(dto.Email ?? string.Empty));

            if (existente is not null)
                return Conflict("E-mail já cadastrado para outro usuário.");

            var entidade = dto.ToEntity();
            await _usuarioAppService.CreateAsync(entidade);
            var criadoDto = entidade.ToDto();

            return CreatedAtAction(nameof(GetById), new { id = entidade.UsuarioId }, criadoDto);
        }

        [HttpPut("{id:int}")]
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> Update(int id, [FromBody] UsuarioDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var existente = await _usuarioAppService.AsTracking().FirstOrDefaultAsync(u => u.UsuarioId == id);

            if (existente is null)
                return NotFound();

            var outroUsuario = _usuarioAppService.GetByEmail(dto.Email);
            if (outroUsuario is not null && outroUsuario.UsuarioId != id)
                return Conflict("E-mail já utilizado por outro usuário.");

            existente.UpdateEntity(dto);
            _usuarioAppService.Update(existente);

            return NoContent();
        }

        [HttpDelete("{id:int}")]
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> Delete(int id)
        {
            var existente = await _usuarioAppService.AsTracking().FirstOrDefaultAsync(u => u.UsuarioId == id);

            if (existente is null) return NotFound();

            _usuarioAppService.Delete(existente);

            return NoContent();
        }
    }
}
