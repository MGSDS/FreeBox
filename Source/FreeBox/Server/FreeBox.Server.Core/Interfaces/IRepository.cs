using FreeBox.Server.Core.Entities;
using File =  FreeBox.Server.Core.Entities.File;

namespace FreeBox.Server.Core.Interfaces;

public interface IRepository
{
    FileStorage Save(File file);
    File GetFile(FileStorage file);
    void DeleteFile(FileStorage file);
}