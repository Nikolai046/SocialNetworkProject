namespace SocialNetwork.Core.Models.ViewModels.DTO;

/// <summary>
/// Представляет модель представления комментария к сообщению.
/// </summary>
public class CommentViewModel
{
    public int CommentId { get; set; }
    public string? Text { get; set; }
    public DateTime CreatedAt { get; set; }
    public bool Deletable { get; set; }
    public string Author { get; set; }
}