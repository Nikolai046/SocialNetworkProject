namespace SocialNetwork.Models;

/// <summary>
/// Класс CommentDto представляет собой объект передачи данных для комментария.
/// Содержит текст комментария и идентификатор сообщения, к которому он относится.
/// </summary>
public class CommentDto
{
    public string Text { get; set; }
    public int MessageId { get; set; }
}