using GestaoSaudeIdosos.Application.Interfaces;
using GestaoSaudeIdosos.Application.Security;
using GestaoSaudeIdosos.Domain.Common.Validation;
using GestaoSaudeIdosos.Domain.Entities;
using GestaoSaudeIdosos.Domain.Interfaces.Services;

namespace GestaoSaudeIdosos.Application.AppServices
{
    public class UsuarioAppService : AppService<Usuario>, IUsuarioAppService
    {
        private readonly IUsuarioService _service;
        private readonly IPasswordHasher _passwordHasher;

        public UsuarioAppService(IUsuarioService service, IPasswordHasher passwordHasher) : base(service)
        {
            _service = service;
            _passwordHasher = passwordHasher;
        }

        public Usuario? GetByEmail(string email) => _service.GetByEmail(email);

        public override async Task CreateAsync(Usuario entity)
        {
            if (entity is null)
                throw new ArgumentNullException(nameof(entity));

            EnsurePasswordMeetsPolicy(entity.Senha);
            entity.Senha = _passwordHasher.Hash(entity.Senha);
            await base.CreateAsync(entity);
        }

        public override void Update(Usuario entity)
        {
            if (entity is null)
                throw new ArgumentNullException(nameof(entity));

            if (!_passwordHasher.IsHashed(entity.Senha))
            {
                EnsurePasswordMeetsPolicy(entity.Senha);
                entity.Senha = _passwordHasher.Hash(entity.Senha);
            }

            base.Update(entity);
        }

        public bool VerifyPassword(Usuario usuario, string senha)
        {
            if (usuario is null)
                throw new ArgumentNullException(nameof(usuario));

            if (string.IsNullOrWhiteSpace(usuario.Senha))
                return false;

            if (_passwordHasher.IsHashed(usuario.Senha))
                return _passwordHasher.Verify(usuario.Senha, senha);

            var matches = string.Equals(usuario.Senha, senha, StringComparison.Ordinal);

            if (matches)
            {
                usuario.Senha = _passwordHasher.Hash(senha);
                base.Update(usuario);
            }

            return matches;
        }

        private static void EnsurePasswordMeetsPolicy(string senha)
        {
            if (!PasswordPolicy.IsValid(senha))
                throw new ArgumentException(PasswordPolicy.BuildRequirementDescription(), nameof(Usuario.Senha));
        }
    }
}
