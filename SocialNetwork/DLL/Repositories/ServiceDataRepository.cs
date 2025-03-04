using SocialNetwork.DLL.DB;
using SocialNetwork.DLL.Entities;

namespace SocialNetwork.DLL.Repositories;

/// <summary>
/// Репозиторий для управления служебными данными программы, предоставляющий функциональность базового репозитория.
/// </summary>
/// <param name="db">Контекст базы данных, используемый для доступа к данным.</param>
public class ServiceDataRepository : Repository<ServiceData>
{
    public ServiceDataRepository(ApplicationDbContext db) : base(db)
    {
    }
}