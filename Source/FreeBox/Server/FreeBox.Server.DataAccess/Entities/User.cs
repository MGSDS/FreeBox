using System.ComponentModel.DataAnnotations;

namespace FreeBox.Server.DataAccess.Entities;

public class User
{
     public User(string login, string password, string role)
     {
          Login = login;
          Password = password;
          Role = role;
          Files = new List<File>();
     }
     private User(){}

     public List<File> Files { get; private init; }
     
     [Key]
     public string Login { get; private init; }
     public string Password { get; private init; }
     public string Role { get; private init; }
}