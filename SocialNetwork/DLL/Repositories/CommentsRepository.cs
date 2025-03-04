using SocialNetwork.DLL.DB;
using SocialNetwork.DLL.Entities;

namespace SocialNetwork.DLL.Repositories;

/// <summary>
/// Репозиторий для работы с комментариями в базе данных.
/// </summary>
/// <param name="db">Контекст базы данных, с которым будет работать репозиторий.</param>
public class CommentsRepository : Repository<Comment>
{
    public CommentsRepository(ApplicationDbContext db) : base(db)
    {
    }
}