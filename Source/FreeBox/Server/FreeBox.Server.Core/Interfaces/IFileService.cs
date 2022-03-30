using FreeBox.Server.Core.Entities;
using File = FreeBox.Server.Core.Entities.File;
using FileInfo = FreeBox.Server.Core.Entities.FileInfo;

namespace FreeBox.Server.Core.Interfaces;

public interface IFileService
{
    FileInfo SaveFile(File file, User user);
    File GetFile(FileInfo file);
    List<FileInfo> GetUserFiles(User user);
    void DeleteFile(FileInfo file);
}