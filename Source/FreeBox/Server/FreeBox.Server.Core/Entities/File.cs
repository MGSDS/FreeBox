using FreeBox.Server.DataAccess;

namespace FreeBox.Server.Core.Entities;

public class File : IDisposable
{
    public Stream Content { get; }
    public FileInfo FileInfo { get; }

    public File(FileInfo fileInfo, Stream content)
    {
        Content = new MemoryStream();
        long pos = content.Position;
        content.Position = 0;
        content.CopyTo(Content);
        content.Position = pos;
        FileInfo = fileInfo;

    }

    public void Dispose()
    {
        Content.Dispose();
    }
}