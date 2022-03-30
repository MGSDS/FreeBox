using FreeBox.Server.Core.Models;
using File = FreeBox.Server.Core.Models.File;
using FileInfo = FreeBox.Server.Core.Models.FileInfo;

namespace FreeBox.Server.Core.Interfaces;

public interface IFileService
{
    FileInfo SaveFile(File file, string login);
    File GetFile(FileInfo file);
    List<FileInfo> GetUserFiles(string login);
    void DeleteFile(FileInfo file);
}