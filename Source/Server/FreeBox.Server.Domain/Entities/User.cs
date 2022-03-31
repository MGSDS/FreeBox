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
     private User(){}

     public List<FileContainer> Files { get; private init; }
     
     [Key]
     public string Login { get; private init; }
     public string Password { get; private init; }
     public string Role { get; private init; }
}