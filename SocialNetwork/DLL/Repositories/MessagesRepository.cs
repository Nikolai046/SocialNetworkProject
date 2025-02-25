﻿using SocialNetwork.DLL.DB;
using SocialNetwork.DLL.Entities;

namespace SocialNetwork.DLL.Repositories;

public class FriendsRepository : Repository<Friend>
{
    public FriendsRepository(ApplicationDbContext db) : base(db)
    {
    }

}