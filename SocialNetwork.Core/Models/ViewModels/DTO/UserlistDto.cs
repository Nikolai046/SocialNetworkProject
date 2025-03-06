namespace SocialNetwork.Core.Models.ViewModels.DTO;

/// <summary>
/// Класс UserlistDto представляет собой объект передачи данных, содержащий информацию о пользователе и статусе дружбы.
/// </summary>
/// <remarks>
/// Свойство 'IsMyFriend' указывает, является ли данный пользователь другом.
/// </remarks>
public class UserlistDto
{
    public string UserId { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string FullName { get; set; }
    public string Email { get; set; }
    public string Image { get; set; }
    public DateTime BirthDate { get; set; }
    public string Status { get; set; }
    public string About { get; set; }
    public bool IsMyFriend { get; set; }
}