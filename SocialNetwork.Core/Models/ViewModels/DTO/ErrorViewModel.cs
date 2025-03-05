namespace SocialNetwork.Core.Models.ViewModels.DTO;

/// <summary>
/// ������ ������������� ������, ���������� ���������� � �������.
/// </summary>
/// <param name="RequestId">������������� �������, ������� ����� ���� null.</param>

public class ErrorViewModel
{
    public string? RequestId { get; set; }

    public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
}