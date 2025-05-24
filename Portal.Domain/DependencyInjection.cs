using Microsoft.Extensions.DependencyInjection;
using GestaoSaudeIdosos.Domain.Interfaces.Services;
using GestaoSaudeIdosos.Domain.Services;

namespace GestaoSaudeIdosos.Domain
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddDomainServices(this IServiceCollection services)
        {
            services.AddScoped(typeof(IService<>), typeof(Service<>));
            services.AddScoped<IUsuarioService, UsuarioService>();

            return services;
        }
    }
}