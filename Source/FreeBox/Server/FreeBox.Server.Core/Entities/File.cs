using FreeBox.Server.DataAccess;

namespace FreeBox.Server.Core.Entities;

public class File : IDisposable
{
    public File(FileInfo fileInfo, Stream content)
    {
        Content = new MemoryStream();
        var pos = content.Position;
        content.Position = 0;
        content.CopyTo(Content);
        content.Position = pos;
        FileInfo = fileInfo;

    }
    
    public MemoryStream Content { get; }
    public FileInfo FileInfo { get; }

    public byte[] BinaryContent => Content.ToArray();

    public void Dispose()
    {
        Content.Dispose();
    }
}