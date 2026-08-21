using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Pdv.Infra.Data.Context;

namespace Pdv.Infra.Ioc;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        //DbContext
        services.AddDbContext<PdvDbContext>(opt =>
        {
            opt.UseNpgsql(configuration.GetConnectionString("DefaultConnection"),
                sqlOpt => sqlOpt.MigrationsAssembly(typeof(PdvDbContext).Assembly.FullName));
        });

        return services;
    }
}
