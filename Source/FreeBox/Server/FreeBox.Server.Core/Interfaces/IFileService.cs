using FreeBox.Server.Core.Models;
using File = FreeBox.Server.Core.Models.File;
using FileInfo = FreeBox.Server.Core.Models.FileInfo;

namespace FreeBox.Server.Core.Interfaces;

public interface IFileService
{
    FileInfo Save(File file, string login);
    File Find(string login, Guid fileId);
    List<FileInfo> Find(string login);
    void Delete(string login, Guid fileId);
}