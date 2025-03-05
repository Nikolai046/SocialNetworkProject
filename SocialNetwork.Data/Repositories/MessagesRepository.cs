using SocialNetwork.Data;
using SocialNetwork.Data.Entities;

namespace SocialNetwork.Data.Repositories;

/// <summary>
/// Репозиторий для работы с данными о друзьях в базе данных.
/// </summary>
/// <param name="db">Контекст базы данных, с которым будет работать репозиторий.</param>
public class FriendsRepository : Repository<Friend>
{
    public FriendsRepository(ApplicationDbContext db) : base(db)
    {
    }
}