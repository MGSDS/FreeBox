using FreeBox.DataAccess;
using FreeBox.Server.Core.Interfaces;
using FreeBox.Server.Domain.Entities;
using FreeBox.Shared.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace FreeBox.Server.Core.Services;

public class FileService : IFileService
{
    private readonly FreeBoxContext _context;
    private readonly ICompressionAlgorithm _compressionAlgorithm;

    public FileService(FreeBoxContext context, ICompressionAlgorithm compressionAlgorithm)
    {
        _context = context;
        _compressionAlgorithm = compressionAlgorithm;
    }

    public ContainerInfo SaveFile(FileContainer container, string login)
    {
        User client = FindUserWithFiles(login);
        using ContainerData compressed = _compressionAlgorithm.Compress(container.Data);
        using var model = new FileContainer(
            container.Info.Duplicate(),
            compressed);
        client.Files.Add(model);
        _context.SaveChanges();

        return model.Info;
    }

    public FileContainer GetFile(Guid fileInfoId)
    {
        FileContainer? fileContainer = _context.Files
            .Include(x => x.Data)
            .Include(x => x.Info)
            .FirstOrDefault(x => x.Info.Id == fileInfoId);
        if (fileContainer == null)
            throw new FileNotFoundException();
        ContainerData decompressed = _compressionAlgorithm.Decompress(fileContainer.Data);
        return new FileContainer(fileContainer.Info.Duplicate(), decompressed);
    }

    public List<ContainerInfo> FindUserFiles(string login)
    {
        User client = FindUserWithFiles(login);
        return client.Files
            .Select(x => x.Info)
            .ToList();
    }

    public void DeleteFile(Guid fileInfoId)
    {
        FileContainer? entry = _context.Files
            .Include(x => x.Data)
            .Include(x => x.Info)
            .FirstOrDefault(x => x.Info.Id == fileInfoId);
        if (entry == null)
        {
            throw new FileNotFoundException();
        }

        _context.Files.Remove(entry);
        _context.FileInfos.Remove(entry.Info);
        _context.Blobs.Remove(entry.Data);
        _context.SaveChanges();
        entry.Dispose();
    }

    private User FindUserWithFiles(string login)
    {
        User? client = _context.Users
            .Include(x => x.Files)
            .ThenInclude(x => x.Info)
            .FirstOrDefault(x => x.Login == login);
        if (client is null)
            throw new UserNotFoundException();
        return client;
    }
}