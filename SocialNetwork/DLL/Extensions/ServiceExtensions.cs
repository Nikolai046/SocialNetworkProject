using SocialNetwork.DLL.Repositories;
using SocialNetwork.DLL.UoW;

namespace SocialNetwork.DLL.Extensions
{
    /// <summary>
    /// Содержит методы расширения для конфигурации сервисов в приложении.
    /// </summary>
    public static class ServiceExtensions
    {
        /// <summary>
        /// Расширяет коллекцию сервисов, добавляя реализацию UnitOfWork.
        /// </summary>
        /// <param name="services">Коллекция сервисов, к которой добавляется UnitOfWork.</param>
        /// <returns>
        /// Возвращает измененную коллекцию сервисов с добавленной реализацией UnitOfWork.
        /// </returns>
        public static IServiceCollection AddUnitOfWork(this IServiceCollection services)
        {
            services.AddScoped<IUnitOfWork, UnitOfWork>();

            return services;
        }

        /// <summary>
        /// Регистрирует репозиторий для указанного типа сущности TEntity с использованием указанного интерфейса IRepository в контейнере зависимостей.
        /// </summary>
        /// <typeparam name="TEntity">Тип сущности, для которой регистрируется репозиторий.</typeparam>
        /// <typeparam name="IRepository">Тип интерфейса репозитория, который будет использоваться для взаимодействия с сущностями TEntity.</typeparam>
        /// <param name="services">Коллекция сервисов, к которой будет добавлен репозиторий.</param>
        /// <returns>
        /// Возвращает измененный объект IServiceCollection, содержащий новую регистрацию репозитория.
        /// </returns>
        public static IServiceCollection AddCustomRepository<TEntity, IRepository>(this IServiceCollection services)
            where TEntity : class
            where IRepository : class, IRepository<TEntity>
        {
            services.AddScoped<IRepository<TEntity>, IRepository>();

            return services;
        }
    }
}