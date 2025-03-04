using SocialNetwork.DLL.Entities;

namespace SocialNetwork.Models.ViewModels.Account;

public class UserViewModel
{
    public User _user;
    public List<Friend> Friends { get; set; }

    public UserViewModel(User user, List<MessageViewModel> messages = null, List<Friend> friends = null)
    {
        _user = user;
        Friends = friends ?? [];
    }
}