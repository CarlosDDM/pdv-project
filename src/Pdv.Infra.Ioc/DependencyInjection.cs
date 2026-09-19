using Amazon.Runtime;
using Amazon.S3;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Pdv.Application.Interfaces;
using Pdv.Domain.Interfaces;
using Pdv.Infra.Data.Context;
using Pdv.Infra.Data.Repositories;
using Pdv.Infra.Data.Storage;

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


        services.AddSingleton<IAmazonS3>(sp =>
        {
            var s3Config = new AmazonS3Config
            {
                ServiceURL = configuration["AWS:S3:ServiceURL"],
                ForcePathStyle = false,
                AuthenticationRegion = configuration["AWS:S3:Region"],
                RequestChecksumCalculation = RequestChecksumCalculation.WHEN_REQUIRED,
                ResponseChecksumValidation = ResponseChecksumValidation.WHEN_REQUIRED
            };

            return new AmazonS3Client(
                configuration["AWS:S3:AccessKey"],
                configuration["AWS:S3:SecretKey"],
                s3Config);
        });

        services.AddScoped<IProductRepository, ProductRepository>();
        services.AddScoped<IStorageService, S3StorageService>();


        return services;
    }
}
