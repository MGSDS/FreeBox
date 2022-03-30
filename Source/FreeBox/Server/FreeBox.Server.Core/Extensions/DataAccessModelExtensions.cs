using FreeBox.Server.Core.Models;
using FreeBox.Server.DataAccess.Entities;
using File = FreeBox.Server.Core.Models.File;
using FileInfo = FreeBox.Server.Core.Models.FileInfo;
using User = FreeBox.Server.DataAccess.Entities.User;

namespace FreeBox.Server.Core.Extensions;

public static class DataAccessModelExtensions
{
    public static FileInfo ToFileInfo(this DataAccess.Entities.File file)
    {
        return new FileInfo(file.Id, file.Name, file.Size, file.SaveDateTime);
    }

    public static File ToFile(this DataAccess.Entities.File file)
    {
        var stream = new MemoryStream(file.Blob.Binary);
        var res = new File(file.ToFileInfo(), stream);
        stream.Dispose();
        return res;
    }

    public static Models.User ToUser(this User user)
    {
        return new Models.User(user.Login, user.Password, user.Role);
    }
}