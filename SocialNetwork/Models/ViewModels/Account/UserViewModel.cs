using SocialNetwork.DLL.Entities;

namespace SocialNetwork.Models.ViewModels.Account;

public class UserViewModel
{
    public User _user;
    public UserViewModel(User user)
    {
        _user = user;
    }
}