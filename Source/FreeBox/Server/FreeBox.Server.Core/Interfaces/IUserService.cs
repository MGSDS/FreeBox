using FreeBox.Server.Core.Models;

namespace FreeBox.Server.Core.Interfaces;

public interface IUserService
{
    List<User> GetUsers();
    User GetUser(string login);
    User AddUser(string login, string password);
    void DeleteUser(string login);
}