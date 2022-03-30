using FreeBox.Server.Core.Extensions;
using FreeBox.Server.Core.Interfaces;
using FreeBox.Server.Core.Models;
using FreeBox.Server.DataAccess;
using FreeBox.Server.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;

namespace FreeBox.Server.Core.Services;

public class UserService : IUserService
{
    private BusinessContext _businessContext;
    private ILogger<UserService> _logger;
    private IFileService _fileService;

    public UserService(BusinessContext businessContext, ILogger<UserService> logger, IFileService fileService)
    {
        _businessContext = businessContext;
        _logger = logger;
        _fileService = fileService;
    }

    public List<User> GetUsers()
    {
        return _businessContext.Clients
            .Select(x => new User(x.Id, x.Name))
            .ToList();
    }

    public User GetUser(Guid userId)
    {
        try
        {
            return _businessContext.Clients
                .Where(x => x.Id == userId)
                .Select(x => new User(x.Id, x.Name))
                .First();
        }
        catch (Exception e)
        {
            _logger.Log(LogLevel.Error, $"User can not be returned {e.Message}");
            throw;
        }
    }

    public User AddUser(string name)
    {
        var client = new ClientModel(name);
        _businessContext.Clients.Add(client);
        _businessContext.SaveChanges();
        _logger.Log(LogLevel.Information, $"Client {client.Id} created");
        return new User(client.Id, client.Name);
    }

    public void DeleteUser(Guid id)
    {
        if (!_businessContext.Clients.Any(x => x.Id == id))
            _logger.Log(LogLevel.Error, $"Can not delete user, no user with id: {id} found");

        var record = _businessContext.Clients
            .First(x => x.Id == id);

        _fileService.GetUserFiles(record.ToUser()).ForEach(x => _fileService.DeleteFile(x));
        _businessContext.Clients.Remove(record);

        _businessContext.Clients.Remove(record);
        _businessContext.SaveChanges();
        _logger.Log(LogLevel.Information, $"User with id {id} deleted");
    }
}