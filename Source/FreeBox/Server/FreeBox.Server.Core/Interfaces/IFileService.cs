using FreeBox.Server.Core.Models;
using File = FreeBox.Server.Core.Models.File;
using FileInfo = FreeBox.Server.Core.Models.FileInfo;

namespace FreeBox.Server.Core.Interfaces;

public interface IFileService
{
    FileInfo SaveFile(File file, User user);
    File GetFile(FileInfo file);
    List<FileInfo> GetUserFiles(User user);
    void DeleteFile(FileInfo file);
}