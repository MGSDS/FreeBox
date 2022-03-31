using FreeBox.Server.Core.Exceptions;
using FreeBox.Server.Core.Extensions;
using FreeBox.Server.Core.Interfaces;
using FreeBox.Server.DataAccess;
using FreeBox.Server.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;
using File = FreeBox.Server.Core.Models.File;
using FileInfo = FreeBox.Server.Core.Models.FileInfo;
using FileNotFoundException = FreeBox.Server.Core.Exceptions.FileNotFoundException;
using User = FreeBox.Server.DataAccess.Entities.User;

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

    public FileInfo Save(File file, string login)
    {
        User client = FindUserWithFiles(login);
        File compressed = _compressionAlgorithm.Compress(file);
        var model = new DataAccess.Entities.File(
            file.FileInfo.Name,
            file.FileInfo.Size,
            file.FileInfo.SaveDate,
            new Blob(compressed.BinaryContent)
        );
        client.Files.Add(model);
        _context.SaveChanges();
        compressed.Dispose();

        return model.ToFileInfo();
    }

    public File Find(string login, Guid fileId)
    {
        if (Find(login).All(x => x.Id != fileId))
            throw new FileNotFoundException();
        var fileModel = _context.Files
            .Include(x => x.Blob)
            .First(x => x.Id == fileId);
        File decompressed = _compressionAlgorithm.Decompress(fileModel.ToFile());
        return decompressed;
    }

    public List<FileInfo> Find(string login)
    {
        User client = FindUserWithFiles(login);
        return client.Files
            .Select(x => x.ToFileInfo())
            .ToList();
    }

    public void Delete(string login, Guid fileId)
    {
        var entry = _context.Files
            .Include(x => x.Blob)
            .First(x => x.Id == fileId);
        _context.Files.Remove(entry);
        _context.Blobs.Remove(entry.Blob);
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