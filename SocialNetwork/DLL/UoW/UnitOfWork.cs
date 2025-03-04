using Microsoft.EntityFrameworkCore.Infrastructure;
using SocialNetwork.DLL.DB;
using SocialNetwork.DLL.Repositories;

namespace SocialNetwork.DLL.UoW
{
    /// <summary>
    /// Класс UnitOfWork представляет паттерн "Единица работы", который используется для группировки одного или нескольких
    /// операций (например, вставки, удаления, обновления) в одну транзакцию, чтобы обеспечить согласованность данных и выполнение
    /// всех операций или откат изменений в случае ошибки.
    /// </summary>
    public class UnitOfWork : IUnitOfWork
    {
        private ApplicationDbContext _appContext;
        private Dictionary<Type, object> _repositories;
        private bool _disposed = false;

        /// <summary>
        /// Инициализирует новый экземпляр класса UnitOfWork, который использует предоставленный контекст приложения.
        /// </summary>
        /// <param name="app">Контекст приложения, который будет использоваться для работы с базой данных.</param>
        /// <exception cref="ArgumentNullException">Исключение, которое выбрасывается, если предоставленный контекст приложения равен null.</exception>
        public UnitOfWork(ApplicationDbContext app)
        {
            _appContext = app ?? throw new ArgumentNullException(nameof(app));
            _repositories = new Dictionary<Type, object>();
        }

        /// <summary>
        /// Освобождает все ресурсы, используемые текущим экземпляром класса, и подавляет финализацию.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Выполняет освобождение ресурсов, используемых объектом, различая управляемые и неуправляемые ресурсы.
        /// </summary>
        /// <param name="disposing">Если true, метод вызван явно и должны быть освобождены управляемые ресурсы. Если false,
        /// метод вызван из финализатора и не должны освобождаться управляемые ресурсы.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    // Освобождаем управляемые ресурсы
                    _appContext?.Dispose();
                }
                // Освобождаем неуправляемые ресурсы
                _disposed = true;
            }
        }

        /// <summary>
        /// Получает репозиторий для типа TEntity, позволяя опционально использовать пользовательский репозиторий.
        /// </summary>
        /// <param name="hasCustomRepository">Если true, метод сначала попытается получить пользовательский репозиторий из контекста приложения.</param>
        /// <typeparam name="TEntity">Тип сущности, для которой требуется репозиторий.</typeparam>
        /// <returns>
        /// Возвращает экземпляр репозитория для указанного типа TEntity. Если пользовательский репозиторий доступен и параметр
        /// hasCustomRepository установлен в true, возвращается пользовательский репозиторий; в противном случае создается и возвращается стандартный репозиторий.
        /// </returns>
        public IRepository<TEntity> GetRepository<TEntity>(bool hasCustomRepository = true) where TEntity : class
        {
            if (hasCustomRepository)
            {
                var customRepo = _appContext.GetService<IRepository<TEntity>>();
                if (customRepo != null)
                {
                    return customRepo;
                }
            }

            var type = typeof(TEntity);
            if (!_repositories.TryGetValue(type, out object? value))
            {
                value = new Repository<TEntity>(_appContext);
                _repositories[type] = value;
            }

            return (IRepository<TEntity>)value;
        }

        /// <summary>
        /// Сохраняет все изменения, сделанные в контексте базы данных.
        /// </summary>
        /// <param name="ensureAutoHistory">Если установлено в true, автоматически сохраняет историю изменений для всех измененных сущностей.</param>
        /// <returns>
        /// Возвращает количество объектов, записанных в базу данных.
        /// </returns>
        public int SaveChanges(bool ensureAutoHistory = false)
        {
            throw new NotImplementedException();
        }
    }
}