using System.IO.Compression;
using FreeBox.Server.Core.Extensions;
using FreeBox.Server.Core.Interfaces;
using File = FreeBox.Server.Core.Entities.File;

namespace FreeBox.Server.Core.CompressionAlgorithms;

public class ZipCompressor : ICompressionAlgorithm
{
    public File Compress(File data)
    {
        using (var compressedStream = new MemoryStream())
        {
            using (var compressor = new DeflateStream(compressedStream, CompressionMode.Compress, leaveOpen: true))
            {
                data.Content.Position = 0;
                data.Content.CopyTo(compressor);
                compressor.Close();
                var file = new File(data.FileInfo, compressedStream);
                return file;
            }
        }
    }

    public File Decompress(File data)
    {
        data.Content.Position = 0;
        using (var deCompressedStream = new MemoryStream())
        {
            using (var compressor = new DeflateStream(data.Content, CompressionMode.Decompress))
            {
                compressor.CopyTo(deCompressedStream);
                deCompressedStream.Position = 0;
                return new File(data.FileInfo, deCompressedStream);
            }
        }
    }
}