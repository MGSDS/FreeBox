namespace FreeBox.Server.Core.Models;

public class FileStorage
{
    public FileStorage(FileInfo file, FileInfo storage)
    {
        File = file;
        Storage = storage;
        Id = Guid.Empty;
    }
    
    public FileStorage(FileInfo file, FileInfo storage, Guid id)
    {
        File = file;
        Storage = storage;
        Id = id;
    }

    public Guid Id { get; internal set; }
    public FileInfo File { get; }
    public FileInfo Storage { get; }
}