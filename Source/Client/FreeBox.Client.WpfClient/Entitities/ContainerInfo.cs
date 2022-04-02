using System;

namespace FreeBox.Client.WpfClient.Entitities;

public class ContainerInfo
{
    public ContainerInfo(Guid id, string name, long size, DateTime saveDate)
    {
        SaveDate = saveDate;
        Id = id;
        Name = name;
        Size = size;
    }

    public Guid Id { get; } 
    public string Name { get; }
    public long Size { get; }
    public DateTime SaveDate { get; }
    
}