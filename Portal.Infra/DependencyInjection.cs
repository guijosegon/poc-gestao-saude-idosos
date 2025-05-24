using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using GestaoSaudeIdosos.Domain.Interfaces.Repositories;
using GestaoSaudeIdosos.Infra.Contexts;
using GestaoSaudeIdosos.Infra.Repositories;

namespace GestaoSaudeIdosos.Infra
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfraServices(this IServiceCollection services, IConfiguration config)
        {
            if (config.GetConnectionString("DatabaseSqlServer") == "true")
                services.AddDbContext<AppContexto>(options => options.UseNpgsql(config.GetConnectionString("DefaultConnection")));
            else
                services.AddDbContext<AppContexto>(options => options.UseSqlServer(config.GetConnectionString("DefaultConnection")));

            services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
            services.AddScoped<IUsuarioRepository, UsuarioRepository>();

            return services;
        }
    }
}