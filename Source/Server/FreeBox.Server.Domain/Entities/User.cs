using System.ComponentModel.DataAnnotations;

namespace FreeBox.Server.Domain.Entities;

public class User
{
     public User(string login, string password, string role)
     {
          Login = login;
          Password = password;
          Role = role;
          Files = new List<FileContainer>();
     }

#pragma warning disable CS8618
     private User() { }
#pragma warning restore CS8618

     public List<FileContainer> Files { get; private init; }

     [Key]

     // TODO: remove attribute
     public string Login { get; private init; }
     public string Password { get; private init; }
     public string Role { get; private init; }
}