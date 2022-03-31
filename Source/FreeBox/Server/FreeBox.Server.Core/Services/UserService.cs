﻿using FreeBox.Server.Core.Exceptions;
using FreeBox.Server.Core.Extensions;
using FreeBox.Server.Core.Interfaces;
using FreeBox.Server.DataAccess;
using User = FreeBox.Server.Core.Models.User;

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

    public List<User> Get()
    {
        return _freeBoxContext.Users
            .Select(x => x.ToUser())
            .ToList();
    }

    public User Find(string login)
    {
        var res = _freeBoxContext.Users
            .Where(x => x.Login == login)
            .Select(x => x.ToUser())
            .FirstOrDefault();
        if (res is null)
            throw new UserNotFoundException();
        return res;
    }

    public User Find(string login, string password)
    {
        var res =  _freeBoxContext.Users
            .Where(x => x.Login == login && x.Password == password)
            .Select(x => x.ToUser())
            .FirstOrDefault();
        if (res is null)
            throw new UserNotFoundException();
        return res;
    }

    public User Add(string login, string password)
    {
        if (_freeBoxContext.Users.Any(x => x.Login == login))
            throw new InvalidOperationException($"User with login {login} already exists");
        var client = new DataAccess.Entities.User(login, password, "user");
        _freeBoxContext.Users.Add(client);
        _freeBoxContext.SaveChanges();
        _logger.Log(LogLevel.Information, $"Client {login} created");
        return client.ToUser();
    }

    public void Delete(string login)
    {
        if (!_freeBoxContext.Users.Any(x => x.Login == login))
            throw new UserNotFoundException();

        var record = _freeBoxContext.Users
            .First(x => x.Login == login);

        _fileService.Find(login).ForEach(x => _fileService.Delete(login, x.Id));
        _freeBoxContext.Users.Remove(record);

        _freeBoxContext.Users.Remove(record);
        _freeBoxContext.SaveChanges();
        _logger.Log(LogLevel.Information, $"User {login} deleted");
    }
}