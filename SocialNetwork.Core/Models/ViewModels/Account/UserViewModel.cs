using SocialNetwork.DLL.Entities;

namespace SocialNetwork.Models.ViewModels.Account;

/// <summary>
/// Класс представления модели пользователя, содержащий информацию о пользователе и его друзьях.
/// </summary>
/// <param name="user">Объект пользователя, для которого создается модель представления.</param>
/// <param name="messages">Список сообщений, связанных с пользователем (необязательный параметр).</param>
/// <param name="friends">Список друзей пользователя (необязательный параметр).</param>
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