using System.ComponentModel;
using FreeBox.Server.Core.Exceptions;
using FreeBox.Server.Core.Interfaces;
using FreeBox.DataAccess;
using FreeBox.Server.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace FreeBox.Server.Core.Services;

public class FileService : IFileService
{
    private FreeBoxContext _context;
    private ICompressionAlgorithm _compressionAlgorithm;

    public FileService(FreeBoxContext context, ICompressionAlgorithm compressionAlgorithm)
    {
        _context = context;
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

    public FileContainer Find(string login, Guid fileId)
    {
        if (Find(login).All(x => x.Id != fileId))
            throw new FileNotFoundException();
        FileContainer fileContainer = _context.Files
            .Include(x => x.Data)
            .FirstOrDefault(x => x.Id == fileId)!;
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

    public void Delete(string login, Guid fileId)
    {
        if (Find(login).All(x => x.Id != fileId))
            throw new FileNotFoundException();
        FileContainer entry = _context.Files
            .Include(x => x.Data)
            .FirstOrDefault(x => x.Id == fileId)!;
        _context.Files.Remove(entry);
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