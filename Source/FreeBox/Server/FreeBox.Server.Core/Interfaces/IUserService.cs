using FreeBox.Server.Core.Models;

namespace FreeBox.Server.Core.Interfaces;

public interface IUserService
{
    List<User> Get();
    User Find(string login);
    User Find(string login, string password);
    User Add(string login, string password);
    void Delete(string login);
}