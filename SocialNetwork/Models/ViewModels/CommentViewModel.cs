namespace SocialNetwork.Models.ViewModels;

public class CommentViewModel
{
    public int CommentId { get; set; }
    public string? Text { get; set; }
    public DateTime CreatedAt { get; set; }
    public bool Deletable { get; set; }
    public required string Author { get; set; }

}