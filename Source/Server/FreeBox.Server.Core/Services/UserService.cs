using FreeBox.DataAccess;
using FreeBox.Server.Core.Interfaces;
using FreeBox.Server.Domain.Entities;
using FreeBox.Shared.Exceptions;

namespace FreeBox.Server.Core.Services;

public class UserService : IUserService
{
    private FreeBoxContext _freeBoxContext;
    private IFileService _fileService;

    public UserService(FreeBoxContext freeBoxContext, IFileService fileService)
    {
        _freeBoxContext = freeBoxContext;
        _fileService = fileService;
    }

    public List<User> GetUsers()
    {
        return _freeBoxContext.Users
            .ToList();
    }

    public User FindUser(string login)
    {
        User? res = _freeBoxContext.Users
            .FirstOrDefault(x => x.Login == login);
        if (res is null)
            throw new UserNotFoundException();
        return res;
    }

    public User AddUser(string login, string password, string role)
    {
        if (_freeBoxContext.Users.Any(x => x.Login == login))
            throw new UserAlreadyExistsException();
        var client = new User(login, password, role);
        _freeBoxContext.Users.Add(client);
        _freeBoxContext.SaveChanges();
        return client;
    }

    public void DeleteUser(string login)
    {
        if (!_freeBoxContext.Users.Any(x => x.Login == login))
            throw new UserNotFoundException();

        User record = _freeBoxContext.Users
            .First(x => x.Login == login);

        _fileService.FindUserFiles(login).ForEach(x => _fileService.DeleteFile(x.Id));
        _freeBoxContext.Users.Remove(record);
        _freeBoxContext.SaveChanges();
    }
}