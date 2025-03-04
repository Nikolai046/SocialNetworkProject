using Microsoft.EntityFrameworkCore;
using SocialNetwork.DLL.DB;

namespace SocialNetwork.DLL.Repositories;

public class Repository<T> : IRepository<T> where T : class
{
    protected DbContext _db;

    public DbSet<T> Set { get; private set; }

    public Repository(ApplicationDbContext db)

    {
        _db = db;
        var set = _db.Set<T>();
        set.Load();

        Set = set;
    }

    public async Task<int> CreateAsync(T item)
    {
        Set.Add(item);
        return await _db.SaveChangesAsync();
    }

    public async Task<int> DeleteAsync(T item)
    {
        Set.Remove(item);
        return await _db.SaveChangesAsync();
    }

    public async Task<T?> GetAsync(int id)
    {
        return await Set.FindAsync(id);
    }

    public IQueryable<T> GetAll()
    {
        return Set;
    }

    public async Task<int> UpdateAsync(T item)
    {
        Set.Update(item);
        return await _db.SaveChangesAsync();
    }

}