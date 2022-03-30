namespace FreeBox.Server.DataAccess.Entities;

public class FileModel
{
    public FileModel(string name, long size, DateTime saveDateTime, Blob blob)
    {
        Name = name;
        Size = size;
        SaveDateTime = saveDateTime;
        Id = Guid.Empty;
        Blob = blob;
    }
    
    private FileModel() { }
    
    public Guid Id { get; private init; }
    public string Name { get; private init; }
    public long Size { get; private init; }
    public DateTime SaveDateTime { get; private init; }
    public Blob Blob { get; private init; }
}