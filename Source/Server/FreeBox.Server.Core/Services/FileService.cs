using System.ComponentModel;
using FreeBox.Server.Core.Interfaces;
using FreeBox.DataAccess;
using FreeBox.Server.Domain.Entities;
using FreeBox.Shared.Exceptions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace FreeBox.Server.Core.Services;

public class FileService : IFileService
{
    private FreeBoxContext _context;
    private ICompressionAlgorithm _compressionAlgorithm;
    private ILogger<FileService> _logger;

    public FileService(FreeBoxContext context, ILogger<FileService> logger, ICompressionAlgorithm compressionAlgorithm)
    {
        _context = context;
        _logger = logger;
        _compressionAlgorithm = compressionAlgorithm;
    }

    public ContainerInfo Save(FileContainer container, string login)
    {
        User client = FindUserWithFiles(login);
        var compressed = _compressionAlgorithm.Compress(container.Data);
        var model = new FileContainer(
            container.Info.Duplicate(),
            compressed
        );
        client.Files.Add(model);
        _context.SaveChanges();
        compressed.Dispose();

        return model.Info;
    }

    public FileContainer Find(string login, Guid fileInfoId)
    {
        if (Find(login).All(x => x.Id != fileInfoId))
            throw new FileNotFoundException();
        FileContainer fileContainer = _context.Files
            .Include(x => x.Data)
            .Include(x => x.Info)
             .First(x => x.Info.Id == fileInfoId);
        ContainerData decompressed = _compressionAlgorithm.Decompress(fileContainer.Data);
        return new FileContainer(fileContainer.Info.Duplicate(), decompressed);
    }

    public List<ContainerInfo> Find(string login)
    {
        User client = FindUserWithFiles(login);
        return client.Files
            .Select(x => x.Info)
            .ToList();
    }

    public void Delete(string login, Guid fileInfoId)
    {
        if (Find(login).All(x => x.Id != fileInfoId))
            throw new FileNotFoundException();
        FileContainer entry = _context.Files
            .Include(x => x.Data)
            .Include(x => x.Info)
            .First(x => x.Info.Id == fileInfoId);
        _context.Files.Remove(entry);
        _context.FileInfos.Remove(entry.Info);
        _context.Blobs.Remove(entry.Data);
        _context.SaveChanges();
    }

    private User FindUserWithFiles(string login)
    {
        User? client = _context.Users
            .Include(x => x.Files)
            .FirstOrDefault(x => x.Login == login);
        if (client is null)
            throw new UserNotFoundException();
        return client;
    }
}