using SocialNetwork.DLL.DB;
using SocialNetwork.DLL.Entities;

namespace SocialNetwork.DLL.Repositories;

public class CommentsRepository : Repository<Comment>
{
    public CommentsRepository(ApplicationDbContext db) : base(db)
    {
    }
}