using FreeBox.Shared.Dtos;

namespace FreeBox.Server.Core.Models;

public class User
{
    public User(string login, string password, string role)
    {
        Login = login;
        Password = password;
        Role = role;
    }

    public UserDto ToDto()
    {
        return new UserDto(Login, Role);
    }
    
    public string Login { get; }
    public string Password { get; }
    public string Role { get; }
}