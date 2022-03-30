namespace FreeBox.Server.DataAccess.DatabaseModels;

public class FileModel
{
    public FileModel(ClientModel clientModel, string name, long size, string storageName, DateTime saveDateTime)
    {
        ClientModel = clientModel;
        Name = name;
        Size = size;
        StorageName = storageName;
        SaveDateTime = saveDateTime;
        Id = Guid.Empty;
    }
    
    private FileModel() { }
    
    public Guid Id { get; private init; }
    public ClientModel ClientModel { get; private init; }
    public string Name { get; private init; }
    public long Size { get; private init; }
    public string StorageName { get; private init; }
    
    public DateTime SaveDateTime { get; private init; }
}