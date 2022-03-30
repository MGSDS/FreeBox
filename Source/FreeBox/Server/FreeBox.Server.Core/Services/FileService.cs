using FreeBox.Server.Core.Extensions;
using FreeBox.Server.Core.Interfaces;
using FreeBox.Server.Core.Models;
using FreeBox.Server.DataAccess;
using FreeBox.Server.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;
using File = FreeBox.Server.Core.Models.File;
using FileInfo = FreeBox.Server.Core.Models.FileInfo;
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

    public FileInfo SaveFile(File file, string login)
    {
        User client = _context.Users
                                 .Include(x => x.Files)
                                 .First(x => x.Login == login)
                             ?? throw new InvalidOperationException("No user with such id");
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
    
    public File GetFile(FileInfo file)
    {
        var fileModel = _context.Files
            .Include(x => x.Blob)
            .First(x => x.Id == file.Id);
        File decompressed = _compressionAlgorithm.Decompress(fileModel.ToFile());
        return decompressed;
    }

    public List<FileInfo> GetUserFiles(string login)
    {
        User client = _context.Users
                                 .Include(x => x.Files)
                                 .First(x => x.Login == login)
                             ?? throw new InvalidOperationException("No user with such id");
        return client.Files
            .Select(x => x.ToFileInfo())
            .ToList();
    }
    
    public void DeleteFile(FileInfo file)
    {
        var entry = _context.Files
            .Include(x => x.Blob)
            .First(x => x.Id == file.Id);
        _context.Files.Remove(entry);
        _context.Blobs.Remove(entry.Blob);
        _context.SaveChanges();
    }
}