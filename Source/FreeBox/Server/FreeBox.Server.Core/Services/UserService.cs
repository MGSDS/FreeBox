using FreeBox.Server.Core.Extensions;
using FreeBox.Server.Core.Interfaces;
using FreeBox.Server.Core.Models;
using FreeBox.Server.DataAccess;
using FreeBox.Server.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;

namespace FreeBox.Server.Core.Services;

public class UserService : IUserService
{
    private FreeBoxContext _freeBoxContext;
    private ILogger<UserService> _logger;
    private IFileService _fileService;

    public UserService(FreeBoxContext freeBoxContext, ILogger<UserService> logger, IFileService fileService)
    {
        _freeBoxContext = freeBoxContext;
        _logger = logger;
        _fileService = fileService;
    }

    public List<Models.User> GetUsers()
    {
        return _freeBoxContext.Users
            .Select(x => x.ToUser())
            .ToList();
    }

    public Models.User GetUser(string login)
    {
        try
        {
            return _freeBoxContext.Users
                .Where(x => x.Login == login)
                .Select(x => x.ToUser())
                .First();
        }
        catch (Exception e)
        {
            _logger.Log(LogLevel.Error, $"User can not be returned {e.Message}");
            throw;
        }
    }

    public Models.User AddUser(string login, string password)
    {
        var client = new DataAccess.Entities.User(login, password, "user");
        _freeBoxContext.Users.Add(client);
        _freeBoxContext.SaveChanges();
        _logger.Log(LogLevel.Information, $"Client {login} created");
        return client.ToUser();
    }

    public void DeleteUser(string login)
    {
        if (!_freeBoxContext.Users.Any(x => x.Login == login))
            _logger.Log(LogLevel.Error, $"Can not delete user, no user with id: {login} found");

        var record = _freeBoxContext.Users
            .First(x => x.Login == login);

        _fileService.GetUserFiles(login).ForEach(x => _fileService.DeleteFile(x));
        _freeBoxContext.Users.Remove(record);

        _freeBoxContext.Users.Remove(record);
        _freeBoxContext.SaveChanges();
        _logger.Log(LogLevel.Information, $"User {login} deleted");
    }
}