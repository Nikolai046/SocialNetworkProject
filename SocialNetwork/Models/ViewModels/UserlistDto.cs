using SocialNetwork.DLL.Entities;

namespace SocialNetwork.Models.ViewModels;

/// <summary>
/// Класс UserlistDto представляет собой объект передачи данных, содержащий информацию о пользователе и статусе дружбы.
/// </summary>
/// <remarks>
/// Свойство 'user' хранит информацию о пользователе, а свойство 'IsMyFriend' указывает, является ли данный пользователь другом.
/// </remarks>
public class UserlistDto
{
    public User user { get; set; }
    public bool IsMyFriend { get; set; }
}