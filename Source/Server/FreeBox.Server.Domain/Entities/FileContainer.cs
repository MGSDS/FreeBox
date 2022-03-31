namespace FreeBox.Server.Domain.Entities;

public class FileContainer : IDisposable
{
    public FileContainer(ContainerInfo info, ContainerData data)
    {
        Id = Guid.Empty;
        Info = info;
        Data = data;
    }
    
    private FileContainer() { }
    public Guid Id { get; private init; }
    public ContainerInfo Info { get; init; }
    public ContainerData Data { get; private init; }

    public void Dispose()
    {
        Data.Dispose();
    }
}