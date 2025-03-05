namespace SocialNetwork.Core.Models.ViewModels.DTO;

/// <summary>
/// Модель представления ошибки, содержащая информацию о запросе.
/// </summary>
/// <param name="RequestId">Идентификатор запроса, который может быть null.</param>

public class ErrorViewModel
{
    public string? RequestId { get; set; }

    public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
}