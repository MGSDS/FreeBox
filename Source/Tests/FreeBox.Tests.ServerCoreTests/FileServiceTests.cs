using System;
using System.IO;
using System.Linq;
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

public class FileServiceTests : IDisposable
{
    private FreeBoxContext _context;
    private IFileService _service;
    private User _testUser;
    [SetUp]
    public void Setup()
    {
        var options = new DbContextOptionsBuilder<FreeBoxContext>()
            .UseInMemoryDatabase(databaseName: "Test")
            .Options;
        _context = new FreeBoxContext(options);
        _context.Database.EnsureDeleted();
        _testUser = new User("test", "test", "aboba");
        _context.Users.Add(_testUser);
        _context.SaveChanges();
        _service = new FileService(_context,  new NoCompressor());
    }

    [Test]
    public void SaveFile_FileSaved()
    {

        var rnd = new Random();
        var content = new byte[20];
        rnd.NextBytes(content);
        using var stream = new MemoryStream(content);
        using var testContainerData = new ContainerData(stream);

        using var container =
            new FileContainer(new ContainerInfo("test", testContainerData.Content.Length, DateTime.Now), testContainerData);
        _service.SaveFile(container, _testUser.Login);
        Assert.IsNotEmpty(_context.Files.ToList());
        Assert.IsNotEmpty(
            _context.Users
                .Include(x => x.Files)
                .First(x => x.Login == _testUser.Login)
                .Files);
        Assert.True(_context.Files.Include(x => x.Data).First().Data.Content.ToArray().SequenceEqual(content));
        
    }

    [Test]
    public void GetFile_FileReturned()
    {
        var rnd = new Random();
        var content = new byte[20];
        rnd.NextBytes(content);
        using var stream = new MemoryStream(content);
        using var testContainerData = new ContainerData(stream);

        using var container =
            new FileContainer(new ContainerInfo("test", testContainerData.Content.Length, DateTime.Now), testContainerData);
        var savedInfo = _service.SaveFile(container, _testUser.Login);
        var returnedFile = _service.GetFile(savedInfo.Id);
        Assert.True(content.SequenceEqual(returnedFile.Data.Content.ToArray()));
    }

    [Test]
    public void DeleteFile_FileDeleted()
    {
        var rnd = new Random();
        var content = new byte[20];
        rnd.NextBytes(content);
        using var stream = new MemoryStream(content);
        using var testContainerData = new ContainerData(stream);

        using var container =
            new FileContainer(new ContainerInfo("test", testContainerData.Content.Length, DateTime.Now), testContainerData);
        var savedInfo = _service.SaveFile(container, _testUser.Login);
        _service.DeleteFile(savedInfo.Id);
        Assert.IsEmpty(_context.Blobs);
        Assert.IsEmpty(_context.Files);
        Assert.IsEmpty(_context.FileInfos);
    }
    
    [Test]
    public void DeleteNotExistingFile_FileNotFoundException()
    {
        Assert.Throws<FileNotFoundException>(() => _service.DeleteFile(Guid.NewGuid()));
    }
    
    [Test]
    public void GetNotExistingFile_FileNotFoundException()
    {
        Assert.Throws<FileNotFoundException>(() => _service.GetFile(Guid.NewGuid()));
    }

    [Test]
    public void AddFileByNotExistingUser_UserNotFoundException()
    {
        ContainerData testContainerData;
        var rnd = new Random();
        var content = new byte[20];
        rnd.NextBytes(content);
        using (var stream = new MemoryStream(content))
        {
            testContainerData = new ContainerData(stream);
        }
        using var container =
            new FileContainer(new ContainerInfo("test", testContainerData.Content.Length, DateTime.Now), testContainerData);
        Assert.Throws<UserNotFoundException>(() => _service.SaveFile(container, "NotExists"));
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}