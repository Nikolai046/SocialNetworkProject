using Microsoft.AspNetCore.Identity;

namespace SocialNetwork.Models.Entities;

public class User : IdentityUser
{
    public string FirstName { get; set; }

    public string LastName { get; set; }

    public string MiddleName { get; set; }

    public DateTime BirthDate { get; set; }
}


//public class User
//{
//    public Guid Id { get; set; }
//    public string Name { get; set; }
//    public string LastName { get; set; }
//    public string MiddleName { get; set; }
//    public string Email { get; set; }
//    public string Password { get; set; }
//    public DateTime DateOfBirth { get; set; }
//    public string City { get; set; }
//    public string ProfilePictureUrl { get; set; }
//    public DateTime CreatedAt { get; set; }
//    public DateTime UpdatedAt { get; set; }
//    public ICollection<Post> Posts { get; set; } = new List<Post>();
//    public ICollection<User> Friends { get; set; } = new List<User>();
//    public ICollection<Message> Messages { get; set; } = new List<Message>();
//}

//public class Post
//{
//    public int Id { get; set; }
//    public string Title { get; set; }
//    public string Content { get; set; }
//    public DateTime CreatedAt { get; set; }
//    public string Author { get; set; }
//}

//public class Message
//{
//    public string Content { get; set; }
//    public DateTime Timestamp { get; set; }

//    public Message(string content)
//    {
//        Content = content;
//        Timestamp = DateTime.Now;
//    }
//}