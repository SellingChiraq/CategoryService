using Microsoft.EntityFrameworkCore;
using SellingChiraq.CategoryService.Infrasctructure.Context;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace SellingChiraq.CategoryService.Extensions;
public static class DbContextRegistration
{
    public static IServiceCollection ConfigureDbContext(this IServiceCollection services,IConfiguration configuration)
    {
        services.AddEntityFrameworkSqlServer()
            .AddDbContext<CatalogContext>(options =>
            {
                options.UseSqlServer(configuration["ConnectionsString"],
                                     sqlServerOptionsAction: sqloptions =>
                                     {
                                         sqloptions.MigrationsAssembly(typeof(Program).GetTypeInfo().Assembly.GetName().Name);
                                         sqloptions.EnableRetryOnFailure(maxRetryCount:15,maxRetryDelay:TimeSpan.FromSeconds(30),errorNumbersToAdd:null);
                                     });
            });

        return services;
    }
}
