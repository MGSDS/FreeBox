namespace FreeBox.Server.Domain.Entities;

public class ContainerInfo
{
    public ContainerInfo(string name, long size, DateTime saveDateTime)
    {
        Id = Guid.Empty;
        Name = name;
        Size = size;
        SaveDateTime = saveDateTime;
    }

    private ContainerInfo(){ }

    public Guid Id { get; private init; }
    public string Name { get; private init; }
    public long Size { get; private init; }
    public DateTime SaveDateTime { get; private init; }
    
    public ContainerInfo Duplicate()
    {
        return new ContainerInfo(Name, Size, SaveDateTime);
    }
}