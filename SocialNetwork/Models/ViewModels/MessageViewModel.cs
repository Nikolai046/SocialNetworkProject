using Microsoft.Identity.Client;
using SocialNetwork.DLL.Entities;

namespace SocialNetwork.Models.ViewModels;

public class MessageViewModel
{
    public string? Text { get; set; }
    public string? AuthorFullName { get; set; }
    public DateTime CreatedAt { get; set; }
    public int MessageId { get; set; }
    public bool Deletable { get; set; }
    public IEnumerable<CommentViewModel>? Comments { get; set; }
}