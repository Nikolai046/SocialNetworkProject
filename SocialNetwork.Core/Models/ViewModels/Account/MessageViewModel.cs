using SocialNetwork.Core.Models.ViewModels.DTO;

namespace SocialNetwork.Core.Models.ViewModels.Account;

/// <summary>
/// Класс представления сообщения, содержащий информацию о тексте сообщения,
/// полном имени автора, дате создания, идентификаторе сообщения,
/// возможности удаления и связанных комментариях.
/// </summary>
public class MessageViewModel
{
    public string? Text { get; set; }
    public string? AuthorFullName { get; set; }
    public DateTime CreatedAt { get; set; }
    public int MessageId { get; set; }
    public bool Deletable { get; set; }
    public IEnumerable<CommentViewModel>? Comments { get; set; }
}