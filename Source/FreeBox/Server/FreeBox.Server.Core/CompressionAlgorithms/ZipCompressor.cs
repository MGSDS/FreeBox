using System.IO.Compression;
using FreeBox.Server.Core.Interfaces;

namespace FreeBox.Server.Core.CompressionAlgorithms;

public class ZipCompressor : ICompressionAlgorithm
{
    public Stream Compress(Stream data)
    {
        var output = new MemoryStream();
        using var stream = new DeflateStream(output, CompressionLevel.Optimal, leaveOpen: true);
        data.Position = 0;
        data.CopyTo(stream);
        return output;
    }

    public Stream Decompress(Stream data)
    {
        var output = new MemoryStream();
        using var stream = new DeflateStream(data, CompressionMode.Decompress, leaveOpen: true);
        stream.CopyTo(output);
        return output;
    }
}