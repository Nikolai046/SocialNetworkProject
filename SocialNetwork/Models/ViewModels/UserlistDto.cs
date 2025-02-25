using SocialNetwork.DLL.Entities;

namespace SocialNetwork.Models.ViewModels;

public class UserlistDto
{
    public User user { get; set; }
    public bool IsMyFriend { get; set; }
}