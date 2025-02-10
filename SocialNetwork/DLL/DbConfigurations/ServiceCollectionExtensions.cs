using SocialNetwork.DLL.Repositories;

namespace SocialNetwork.DLL.DbConfigurations;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddCustomRepository<TEntity, TRepository>(this IServiceCollection services)
        where TEntity : class
        where TRepository : class, IRepository<TEntity>
    {
        services.AddScoped<IRepository<TEntity>, TRepository>();

        return services;
    }
    public static IServiceCollection AddUnitOfWork(this IServiceCollection services)
    {
        //services.AddScoped<IUnitOfWork, UnitOfWork>();
        return services;
    }
}