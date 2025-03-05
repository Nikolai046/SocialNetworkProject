using SocialNetwork.Data;
using SocialNetwork.Data.Entities;

namespace SocialNetwork.Data.Repositories;

/// <summary>
/// Репозиторий для работы с сообщениями в базе данных.
/// </summary>
/// <param name="db">Контекст базы данных, с которым будет работать репозиторий.</param>
/// <returns>
/// Не возвращает значений, так как это конструктор класса.
/// </returns>
public class MessagesRepository : Repository<Message>
{
    public MessagesRepository(ApplicationDbContext db) : base(db)
    {
    }
}