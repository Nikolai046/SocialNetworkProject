namespace SocialNetwork.Data.Entities;

/// <summary>
/// Класс Friend представляет собой модель для хранения информации о дружеских связях между пользователями.
/// </summary>
public class Friend
{
    public int Id { get; set; }
    public string? UserId { get; set; }
    public User? User { get; set; }
    public string? CurrentFriendId { get; set; }
    public User? CurrentFriend { get; set; }
}