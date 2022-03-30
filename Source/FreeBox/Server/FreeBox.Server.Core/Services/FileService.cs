using FreeBox.Server.Core.Entities;
using FreeBox.Server.Core.Extensions;
using FreeBox.Server.Core.Interfaces;
using FreeBox.Server.DataAccess;
using FreeBox.Server.DataAccess.DatabaseModels;
using Microsoft.EntityFrameworkCore;
using File = FreeBox.Server.Core.Entities.File;
using FileInfo = FreeBox.Server.Core.Entities.FileInfo;

namespace FreeBox.Server.Core.Services;

public class FileService : IFileService
{
    private BusinessContext _context;
    private ICompressionAlgorithm _compressionAlgorithm;

    public FileService(BusinessContext context, ICompressionAlgorithm compressionAlgorithm)
    {
        _context = context;
        _compressionAlgorithm = compressionAlgorithm;
    }

    public FileInfo SaveFile(File file, User user)
    {
        ClientModel client = _context.Clients
                                 .Include(x => x.Files)
                                 .First(x => x.Id == user.Id)
                             ?? throw new InvalidOperationException("No user with such id");
        File compressed = _compressionAlgorithm.Compress(file);
        var model = new FileModel(
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

    public List<FileInfo> GetUserFiles(User user)
    {
        ClientModel client = _context.Clients
                                 .Include(x => x.Files)
                                 .First(x => x.Id == user.Id)
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