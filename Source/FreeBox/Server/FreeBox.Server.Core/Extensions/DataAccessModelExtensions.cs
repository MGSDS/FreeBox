using FreeBox.Server.Core.Models;
using FreeBox.Server.DataAccess.Entities;
using File = FreeBox.Server.Core.Models.File;
using FileInfo = FreeBox.Server.Core.Models.FileInfo;

namespace FreeBox.Server.Core.Extensions;

public static class DataAccessModelExtensions
{
    public static FileInfo ToFileInfo(this FileModel fileModel)
    {
        return new FileInfo(fileModel.Id, fileModel.Name, fileModel.Size, fileModel.SaveDateTime);
    }

    public static File ToFile(this FileModel fileModel)
    {
        var stream = new MemoryStream(fileModel.Blob.Binary);
        var file = new File(fileModel.ToFileInfo(), stream);
        stream.Dispose();
        return file;
    }

    public static User ToUser(this ClientModel clientModel)
    {
        return new User(clientModel.Id, clientModel.Name);
    }
}