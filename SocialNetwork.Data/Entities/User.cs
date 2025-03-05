using Microsoft.AspNetCore.Identity;

namespace SocialNetwork.Data.Entities;

/// <summary>
/// Представляет сущность пользователя в системе, наследуя базовые свойства и методы от IdentityUser.
/// </summary>
public class User : IdentityUser
{
    public string FirstName { get; set; }

    public string LastName { get; set; }

    //public string? MiddleName { get; set; }

    public DateTime BirthDate { get; set; }

    public string Image { get; set; }

    public string Status { get; set; }

    public string About { get; set; }

    public ICollection<Message> SentMessages { get; set; } = new List<Message>();
    public ICollection<Message> ReceivedMessages { get; set; } = new List<Message>();
    public ICollection<Friend> Friends { get; set; } = new List<Friend>();

    public ICollection<Comment>? Comments { get; set; }

    public string GetFullName()
    {
        return $"{FirstName} {LastName}";
    }

    public User()
    {
        Image = "/images/person-unknown.svg";
        Status = "Ура! Я в SocialNetWork!";
        About = "Информация обо мне.";
    }
}