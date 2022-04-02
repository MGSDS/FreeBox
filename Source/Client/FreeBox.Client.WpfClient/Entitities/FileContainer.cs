using System;
using System.IO;

namespace FreeBox.Client.WpfClient.Entitities
{
    internal class FileContainer : IDisposable
    {
        public FileContainer(Stream stream, string name)
        {
            Name = name;
            long pos = stream.Position;
            stream.Position = 0;
            Stream = new MemoryStream();
            stream.CopyTo(Stream);
            stream.Position = pos;
        }

        public MemoryStream Stream { get; }
        public string Name { get; }

        public void Dispose()
        {
            Stream.Dispose();
        }
    }
}
