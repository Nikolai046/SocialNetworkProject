namespace SocialNetwork.Models;

/// <summary>
/// Класс FriendDto представляет объект передачи данных для друга,
/// содержащий идентификатор друга, полное имя и изображение.
/// </summary>
public class FriendDto
{
    public string? FriendId { get; set; }
    public string? FriendFullName { get; set; }
    public string? Image { get; set; }
}