using Microsoft.AspNetCore.Identity;

namespace SocialNetwork.DLL.Entities;

public class User : IdentityUser
{
    public string FirstName { get; set; }

    public string LastName { get; set; }

    public string? MiddleName { get; set; }

    public DateTime BirthDate { get; set; }

    public string Image { get; set; }

    public string Status { get; set; }

    public string About { get; set; }

    public string GetFullName()
    {
        return FirstName + " " + MiddleName + " " + LastName;
    }

    public User()
    {
        Image = "/images/person-unknown.svg";
        Status = "Ура! Я в SocialNetWork!";
        About = "Информация обо мне.";
    }

}
