namespace SocialNetwork.DLL.Entities;

public class Comment
{
    public int Id { get; set; }
    public string? Text { get; set; }
    public DateTime Timestamp { get; set; }

    public string? SenderId { get; set; }
    public User? Sender { get; set; }

    public int InitialMessageId { get; set; }
    public Message? Message { get; set; }

}