using System;
using System.IO;
using System.Linq;
using System.Security.Authentication;
using FreeBox.DataAccess;
using FreeBox.Server.Core.CompressionAlgorithms;
using FreeBox.Server.Core.Interfaces;
using FreeBox.Server.Core.Services;
using FreeBox.Server.Domain.Entities;
using FreeBox.Shared.Exceptions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using NUnit.Framework;

namespace FreeBox.Tests.ServerCoreTests;

public class UserServiceTests
{
    private FreeBoxContext _context;
    private IFileService _fileService;
    private IUserService _userService;

    [SetUp]
    public void Setup()
    {
        var options = new DbContextOptionsBuilder<FreeBoxContext>()
            .UseInMemoryDatabase(databaseName: "Test")
            .Options;
        _context = new FreeBoxContext(options);
        _context.Database.EnsureDeleted();
        _fileService = new FileService(_context, new NullLogger<FileService>(), new ZipCompressor());
        _userService = new UserService(_context, new NullLogger<UserService>(), _fileService);
    }

    [Test]
    public void Add_UserRegistered()
    {
        _userService.Add("test", "test");
        Assert.True(_context.Users.Any());
    }
    
    [Test]
    public void Find_UserReturned()
    {
        _userService.Add("test", "test");
        Assert.IsNotNull(_userService.Find("test"));
        Assert.IsNotNull(_userService.Find("test", "test"));
    }
    
    [Test]
    public void Find_UserNotFoundException()
    {
        Assert.Catch<UserNotFoundException>(() => _userService.Find("test"));
        Assert.Catch<UserNotFoundException>(() => _userService.Find("test", "test"));
    }
    
    [Test]
    public void Find_InvalidCredentialException()
    {
        _userService.Add("test", "test");
        Assert.Catch<InvalidCredentialException>(() => _userService.Find("test", "invalid"));
    }
    
    [Test]
    public void Add_UserAlreadyExistsException()
    {
        _userService.Add("test", "test");
        Assert.Catch<UserAlreadyExistsException>(() => _userService.Add("test", "test"));
    }
    
    [Test]
    public void Delete_UserDeletedWithFiles()
    {
        _userService.Add("test", "test");
        ContainerData testContainerData;
        var rnd = new Random();
        var content = new byte[20];
        rnd.NextBytes(content);
        using (var stream = new MemoryStream(content))
        {
            testContainerData = new ContainerData(stream);
        }
        
        var container =
            new FileContainer(new ContainerInfo("test", testContainerData.Content.Length, DateTime.Now), testContainerData);
        var savedInfo = _fileService.Save(container, "test");
        _userService.Delete("test");
        Assert.IsEmpty(_context.Blobs);
        Assert.IsEmpty(_context.Files);
        Assert.IsEmpty(_context.FileInfos);
        Assert.IsEmpty(_context.Users);
    }
}