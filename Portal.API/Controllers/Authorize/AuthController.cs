using GestaoSaudeIdosos.Application.Interfaces;
using GestaoSaudeIdosos.API.DTOs.Authorize;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Extensions;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Text;

namespace GestaoSaudeIdosos.API.Controllers.Authorize
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IConfiguration _config;
        private readonly IUsuarioAppService _usuarioAppService;

        public AuthController(IUsuarioAppService usuarioAppService, IConfiguration config)
        {
            _usuarioAppService = usuarioAppService;
            _config = config;
        }

        [HttpPost("token")]
        public IActionResult Token([FromBody] LoginDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var login = _usuarioAppService.GetByEmail(dto?.Email);

            if (login is null || login.Senha != dto.Senha)
                return Unauthorized("Credenciais inválidas");

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, login.Email),
                new Claim(ClaimTypes.Role, login.Perfil.GetDisplayName()),
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