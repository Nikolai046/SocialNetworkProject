using SocialNetwork.DLL.Entities;

namespace SocialNetwork.Models.ViewModels.Account;

public class UserViewModel
{
    public User _user;
    public List<MessageViewModel> Messages { get; set; }
    public UserViewModel(User user, List<MessageViewModel> messages=null)
    {
        _user = user;
        Messages = messages ?? [];
    }

}