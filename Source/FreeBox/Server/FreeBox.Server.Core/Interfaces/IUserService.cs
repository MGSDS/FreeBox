using FreeBox.Server.Core.Models;

namespace FreeBox.Server.Core.Interfaces;

public interface IUserService
{
    List<User> GetUsers();
    User GetUser(Guid userId);
    User AddUser(string name);
    void DeleteUser(Guid id);
}