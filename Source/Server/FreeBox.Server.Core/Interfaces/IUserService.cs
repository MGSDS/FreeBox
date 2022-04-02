using FreeBox.Server.Domain.Entities;

namespace FreeBox.Server.Core.Interfaces;

public interface IUserService
{
    List<User> GetUsers();
    User FindUser(string login);
    User AddUser(string login, string password, string role);
    void DeleteUser(string login);
}