namespace SocialNetwork.DLL.Repositories;

public interface IRepository<T> where T : class
{
    IQueryable<T> GetAll();
    Task<T> Get(int id);
    Task<int> Create(T item);
    Task<int> Update(T item);
    Task<int> Delete(T item);
}