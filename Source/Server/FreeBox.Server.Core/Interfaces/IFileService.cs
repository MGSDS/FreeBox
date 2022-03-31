using FreeBox.Server.Domain.Entities;

namespace FreeBox.Server.Core.Interfaces;

public interface IFileService
{
    ContainerInfo Save(FileContainer container, string login);
    FileContainer Find(string login, Guid fileInfoId);
    List<ContainerInfo> Find(string login);
    void Delete(string login, Guid fileInfoId);
}