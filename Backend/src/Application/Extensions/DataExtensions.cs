using Data;
using Data.Interceptors;
using Microsoft.EntityFrameworkCore;

namespace Application.Extensions;

public static class DataExtensions
{
    public static IServiceCollection AddDataService(this IServiceCollection serviceCollection,
        IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection") ??
                               throw new InvalidOperationException("Connection string not found");

        serviceCollection.AddSingleton<SoftDeleteInterceptor>();

        serviceCollection.AddDbContextFactory<ApplicationDbContext>((sp, options) =>
        {
            options.UseNpgsql(connectionString);

            options.AddInterceptors(sp.GetRequiredService<SoftDeleteInterceptor>());
        });

        return serviceCollection;
    }
}
