using Microsoft.EntityFrameworkCore.Infrastructure;
using SocialNetwork.DLL.DB;
using SocialNetwork.DLL.Repositories;

namespace SocialNetwork.DLL.UoW
{
    public class UnitOfWork : IUnitOfWork
    {
        private ApplicationDbContext _appContext;
        private Dictionary<Type, object> _repositories;
        private bool _disposed = false;

        public UnitOfWork(ApplicationDbContext app)
        {
            _appContext = app ?? throw new ArgumentNullException(nameof(app));
            _repositories = new Dictionary<Type, object>();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

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

        public int SaveChanges(bool ensureAutoHistory = false)
        {
            throw new NotImplementedException();
        }
    }
}