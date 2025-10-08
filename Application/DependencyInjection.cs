using GestaoSaudeIdosos.Application.AppServices;
using GestaoSaudeIdosos.Application.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace GestaoSaudeIdosos.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            services.AddScoped(typeof(IAppService<>), typeof(AppService<>));
            services.AddScoped<IUsuarioAppService, UsuarioAppService>();
            services.AddScoped<IPacienteAppService, PacienteAppService>();
            services.AddScoped<IFormularioAppService, FormularioAppService>();

            return services;
        }
    }
}
