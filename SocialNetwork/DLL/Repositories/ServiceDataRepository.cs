using SocialNetwork.DLL.DB;
using SocialNetwork.DLL.Entities;

namespace SocialNetwork.DLL.Repositories;

public class ServiceDataRepository : Repository<ServiceData>
{
    public ServiceDataRepository(ApplicationDbContext db) : base(db)
    {
    }
}