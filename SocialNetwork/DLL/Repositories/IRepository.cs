namespace SocialNetwork.DLL.Repositories;

public interface IRepository<T> where T : class
{
    IQueryable<T> GetAll();
    Task<T?> GetAsync(int id);
    Task<int> CreateAsync(T item);
    Task<int> UpdateAsync(T item);
    Task<int> DeleteAsync(T item);
}