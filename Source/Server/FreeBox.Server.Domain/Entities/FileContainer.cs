namespace FreeBox.Server.Domain.Entities;

public class FileContainer : IDisposable
{
    public FileContainer(ContainerInfo info, ContainerData data)
    {
        Id = Guid.Empty;
        Info = info;
        Data = data;
    }

#pragma warning disable CS8618
    private FileContainer() { }
#pragma warning restore CS8618
    public Guid Id { get; private init; }
    public ContainerInfo Info { get; init; }
    public ContainerData Data { get; private init; }

    public void Dispose()
    {
        Data.Dispose();
    }
}