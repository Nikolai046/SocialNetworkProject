namespace SocialNetwork.Core.Models.ViewModels;

/// <summary>
/// Класс представления модели пользователя.
/// </summary>
public class UserViewModel
{
    public string UserId { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string FullName { get; set; }
    public string Email { get; set; }
    public string Image { get; set; }
    public DateTime BirthDate { get; set; }
    public string Status { get; set; }
    public string About { get; set; }
}