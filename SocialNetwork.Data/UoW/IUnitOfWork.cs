using SocialNetwork.Data.Repositories;

namespace SocialNetwork.Data.UoW;

/// <summary>
/// Интерфейс Unit of Work
/// </summary>
public interface IUnitOfWork : IDisposable
{
    /// <summary>
    /// Сохраняет все изменения, сделанные в контексте базы данных. При указании параметра ensureAutoHistory как true, автоматически сохраняет историю изменений.
    /// </summary>
    /// <param name="ensureAutoHistory">Если установлено в true, метод также сохраняет историю изменений в базе данных. По умолчанию false.</param>
    /// <returns>Возвращает количество измененных объектов в базе данных.</returns>
    int SaveChanges(bool ensureAutoHistory = false);

    /// <summary>
    /// Получает репозиторий для заданного типа сущности TEntity. Позволяет опционально указать, должен ли использоваться пользовательский репозиторий.
    /// </summary>
    /// <param name="hasCustomRepository">Если true, метод пытается получить пользовательский репозиторий для типа TEntity. Если false, возвращает стандартный репозиторий.</param>
    /// <returns>Возвращает объект репозитория для управления сущностями типа TEntity.</returns>
    /// <typeparam name="TEntity">Тип сущности, для которой требуется репозиторий. Должен быть классом.</typeparam>
    IRepository<TEntity> GetRepository<TEntity>(bool hasCustomRepository = true) where TEntity : class;
}