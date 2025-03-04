using Microsoft.EntityFrameworkCore;
using SocialNetwork.DLL.DB;

namespace SocialNetwork.DLL.Repositories;

/// <summary>
/// Представляет репозиторий для работы с объектами типа T, где T является классом.
/// Реализует интерфейс IRepository<T>, предоставляя функциональность для управления данными типа T.
/// </summary>
public class Repository<T> : IRepository<T> where T : class
{
    protected DbContext _db;

    /// <summary>
    /// Получает доступ к набору сущностей типа T, управляемых контекстом базы данных.
    /// </summary>
    public DbSet<T> Set { get; private set; }

    /// <summary>
    /// Конструктор для создания репозитория, который загружает данные из базы данных в локальный контекст.
    /// </summary>
    /// <param name="db">Контекст базы данных, используемый для доступа к данным.</param>
    public Repository(ApplicationDbContext db)

    {
        _db = db;
        var set = _db.Set<T>();
        set.Load();

        Set = set;
    }

    /// <summary>
    /// Асинхронно добавляет элемент в базу данных и сохраняет изменения.
    /// </summary>
    /// <param name="item">Элемент, который необходимо добавить.</param>
    /// <returns>
    /// Возвращает количество измененных строк в базе данных после сохранения.
    /// </returns>
    public async Task<int> CreateAsync(T item)
    {
        Set.Add(item);
        return await _db.SaveChangesAsync();
    }

    /// <summary>
    /// Асинхронно удаляет элемент из набора данных и сохраняет изменения в базе данных.
    /// </summary>
    /// <param name="item">Элемент, который необходимо удалить.</param>
    /// <returns>
    /// Возвращает количество измененных записей в базе данных после удаления элемента.
    /// </returns>
    public async Task<int> DeleteAsync(T item)
    {
        Set.Remove(item);
        return await _db.SaveChangesAsync();
    }

    /// <summary>
    /// Асинхронно извлекает объект из набора данных по указанному идентификатору.
    /// </summary>
    /// <param name="id">Идентификатор объекта для поиска.</param>
    /// <returns>
    /// Возвращает объект типа T, если он найден; в противном случае возвращает значение null.
    /// </returns>
    public async Task<T?> GetAsync(int id)
    {
        return await Set.FindAsync(id);
    }

    /// <summary>
    /// Получает все элементы коллекции как IQueryable.
    /// </summary>
    /// <returns>
    /// Возвращает IQueryable, представляющий коллекцию всех элементов.
    /// </returns>
    public IQueryable<T> GetAll()
    {
        return Set;
    }

    /// <summary>
    /// Асинхронно обновляет сущность в базе данных.
    /// </summary>
    /// <param name="item">Объект сущности, который необходимо обновить.</param>
    /// <returns>
    /// Возвращает количество измененных записей в базе данных.
    /// </returns>
    public async Task<int> UpdateAsync(T item)
    {
        Set.Update(item);
        return await _db.SaveChangesAsync();
    }
}