using GestaoSaudeIdosos.Domain.Interfaces.Services;
using GestaoSaudeIdosos.Domain.Services;
using Microsoft.Extensions.DependencyInjection;

namespace GestaoSaudeIdosos.Domain
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddDomainServices(this IServiceCollection services)
        {
            services.AddScoped(typeof(IService<>), typeof(Service<>));
            services.AddScoped<IUsuarioService, UsuarioService>();
            services.AddScoped<IPacienteService, PacienteService>();
            services.AddScoped<IFormularioService, FormularioService>();
            services.AddScoped<ICampoService, CampoService>();
            services.AddScoped<IGraficoService, GraficoService>();

            return services;
        }
    }
}
