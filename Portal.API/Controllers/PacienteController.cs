using GestaoSaudeIdosos.API.DTOs;
using GestaoSaudeIdosos.API.Mappers;
using GestaoSaudeIdosos.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;

namespace GestaoSaudeIdosos.API.Controllers
{
    [ApiController]
    [Route("api/pacientes")]
    public class PacienteController : ControllerBase
    {
        private readonly IPacienteAppService _pacienteAppService;

        public PacienteController(IPacienteAppService pacienteAppService)
        {
            _pacienteAppService = pacienteAppService;
        }

        [HttpGet]
        [Authorize]
        public async Task<ActionResult<IEnumerable<PacienteDto>>> GetAll()
        {
            var pacientes = await _pacienteAppService.GetAllAsync();
            var dtos = pacientes.Select(p => p.ToDto()).ToList();

            if (!dtos.Any())
                return NotFound();

            return Ok(dtos);
        }

        [HttpGet("{id:int}")]
        [Authorize]
        public async Task<ActionResult<PacienteDto>> GetById(int id)
        {
            var paciente = await _pacienteAppService.GetByIdAsync(id);

            if (paciente is null)
                return NotFound();

            return Ok(paciente.ToDto());
        }

        [HttpPost]
        [Authorize]
        public async Task<ActionResult<PacienteDto>> Create([FromBody] PacienteDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var entidade = dto.ToEntity();
            await _pacienteAppService.CreateAsync(entidade);

            return CreatedAtAction(nameof(GetById), new { id = entidade.PacienteId }, entidade.ToDto());
        }

        [HttpPut("{id:int}")]
        [Authorize]
        public async Task<IActionResult> Update(int id, [FromBody] PacienteDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var existente = await _pacienteAppService.GetByIdAsync(id);

            if (existente is null)
                return NotFound();

            existente.UpdateEntity(dto);
            _pacienteAppService.Update(existente);

            return NoContent();
        }

        [HttpDelete("{id:int}")]
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> Delete(int id)
        {
            var existente = await _pacienteAppService.GetByIdAsync(id);

            if (existente is null)
                return NotFound();

            _pacienteAppService.Delete(existente);

            return NoContent();
        }
    }
}
