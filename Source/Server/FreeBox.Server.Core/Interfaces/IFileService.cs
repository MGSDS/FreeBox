using FreeBox.Server.Domain.Entities;

namespace FreeBox.Server.Core.Interfaces;

public interface IFileService
{
    ContainerInfo SaveFile(FileContainer container, string login);
    FileContainer GetFile(Guid fileInfoId);
    List<ContainerInfo> FindUserFiles(string login);
    void DeleteFile(Guid fileInfoId);
}