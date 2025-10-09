using GestaoSaudeIdosos.API.DTOs.Authorize;
using GestaoSaudeIdosos.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace GestaoSaudeIdosos.API.Controllers.Authorize
{
    [ApiController]
    [Route("api/autenticar")]
    public class AuthController : ControllerBase
    {
        private readonly IConfiguration _config;
        private readonly IUsuarioAppService _usuarioAppService;

        public AuthController(IUsuarioAppService usuarioAppService, IConfiguration config)
        {
            _usuarioAppService = usuarioAppService;
            _config = config;
        }

        [HttpPost]
        public IActionResult Token([FromBody] LoginDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var login = _usuarioAppService.AsQueryable().FirstOrDefault(f => f.Email.Equals(dto.Email ?? string.Empty));

            if (login is null || !login.Ativo)
                return Unauthorized("Credenciais inválidas");

            if (!string.Equals(login.Senha, dto.Senha, StringComparison.Ordinal))
                return Unauthorized("Credenciais inválidas");

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, login.Email),
                new Claim(ClaimTypes.Name, login.Nome),
                new Claim(ClaimTypes.Role, login.Perfil.ToString()),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var jwtSettings = _config.GetSection("Jwt");
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["Key"]!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: jwtSettings["Issuer"],
                audience: jwtSettings["Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(double.Parse(jwtSettings["ExpireMinutes"]!)),
                signingCredentials: creds
            );

            return Ok(new { token = new JwtSecurityTokenHandler().WriteToken(token), expires = token.ValidTo });
        }
    }
}
