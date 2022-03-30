using FreeBox.Server.Core.Entities;
using File = FreeBox.Server.Core.Entities.File;
using FileInfo = System.IO.FileInfo;

namespace FreeBox.Server.Core.Interfaces;

public interface IFileService
{
    FileStorage SaveFile(File file, User user);
    File GetFile(FileStorage fileStorage);
    List<FileStorage> GetUserFiles(User user);
    void DeleteFile(FileStorage fileStorage);
}