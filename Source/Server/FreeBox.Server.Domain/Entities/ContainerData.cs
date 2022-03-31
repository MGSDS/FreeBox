namespace FreeBox.Server.Domain.Entities;

public class ContainerData : IDisposable
{
    public ContainerData(Stream stream)
    {
        Content = new MemoryStream();
        long pos = stream.Position;
        stream.Position = 0;
        stream.CopyTo(Content);
        stream.Position = pos;
        Id = Guid.Empty;
    }
    
    private ContainerData() { }
    
    public MemoryStream Content { get; private init; }
    public Guid Id { get; private init; }

    public void Dispose()
    {
        Content.Dispose();
    }
}