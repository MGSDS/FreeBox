using FreeBox.Server.Core.Entities;
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
    private IRepository _repository;

    public FileService(BusinessContext context, IRepository repository)
    {
        _context = context;
        _repository = repository;
    }

    public FileStorage SaveFile(File file, User user)
    {
        FileStorage storage = _repository.Save(file);
        ClientModel client = _context.Clients
                                 .Include(x => x.Files)
                                 .First(x => x.Id == user.Id)
                             ?? throw new InvalidOperationException("No user with such id");
        var model = new FileModel(
            client,
            storage.File.Name,
            storage.File.Size,
            storage.Storage.Name,
            DateTime.Now);
        _context.Files.Add(model);
        client.Files.Add(model);
        _context.SaveChanges();
        storage.Id = model.Id;

        return storage;
    }
    
    public File GetFile(FileStorage fileStorage)
    {
        var fileModel = _context.Files.First(x => x.Id == fileStorage.Id);
        return _repository.GetFile(
            new FileStorage(
            new FileInfo(fileModel.Name, fileModel.Size, fileModel.SaveDateTime),
            new FileInfo(fileModel.StorageName, 0, fileModel.SaveDateTime),
            fileModel.Id));
    }

    public List<FileStorage> GetUserFiles(User user)
    {
        ClientModel client = _context.Clients
                                 .Include(x => x.Files)
                                 .First(x => x.Id == user.Id)
                             ?? throw new InvalidOperationException("No user with such id");
        return client.Files
            .Select(x =>
                new FileStorage(
                    new Entities.FileInfo(x.Name, x.Size, x.SaveDateTime),
                    null, x.Id))
            .ToList();
    }
    
    public void DeleteFile(FileStorage fileStorage)
    {
        _repository.DeleteFile(fileStorage);
        _context.Files.Remove(_context.Files.First(x => x.Id == fileStorage.Id));
    }
}