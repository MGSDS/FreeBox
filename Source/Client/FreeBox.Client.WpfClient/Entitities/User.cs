namespace FreeBox.Client.WpfClient.Entitities;

public class User
{
    public User(string login, string password, string? token)
    {
        Login = login;
        Password = password;
        Token = token;
    }

    public string Login { get; set; }
    public string Password { get; set; }
    public string? Token { get; set; }
}