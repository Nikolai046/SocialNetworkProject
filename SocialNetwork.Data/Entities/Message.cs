namespace SocialNetwork.Data.Entities;

/// <summary>
/// Представляет собой модель для хранения информации о сообщениях.
/// </summary>
public class Message
{
    public int Id { get; set; }
    public string? Text { get; set; }
    public DateTime Timestamp { get; set; }

    public string? SenderId { get; set; }
    public User? Sender { get; set; }

    public string? RecipientId { get; set; }
    public User? Recipient { get; set; }

    public ICollection<Comment> Comments { get; set; } = new List<Comment>();
}