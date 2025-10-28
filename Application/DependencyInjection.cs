using GestaoSaudeIdosos.Application.AppServices;
using GestaoSaudeIdosos.Application.Interfaces;
using GestaoSaudeIdosos.Application.Security;
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
            services.AddScoped<IFormularioResultadoAppService, FormularioResultadoAppService>();
            services.AddScoped<ICampoAppService, CampoAppService>();
            services.AddScoped<IGraficoAppService, GraficoAppService>();
            services.AddSingleton<IPasswordHasher, Argon2PasswordHasher>();

            return services;
        }
    }
}
