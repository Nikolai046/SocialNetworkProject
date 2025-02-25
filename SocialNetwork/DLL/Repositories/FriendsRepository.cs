using SocialNetwork.DLL.DB;
using SocialNetwork.DLL.Entities;

namespace SocialNetwork.DLL.Repositories;

public class MessagesRepository : Repository<Message>
{
    public MessagesRepository(ApplicationDbContext db) : base(db)
    {
    }

}