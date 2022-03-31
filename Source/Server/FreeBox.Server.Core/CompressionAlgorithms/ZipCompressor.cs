using System.IO.Compression;
using FreeBox.Server.Core.Interfaces;
using FreeBox.Server.Domain.Entities;

namespace FreeBox.Server.Core.CompressionAlgorithms;

public class ZipCompressor : ICompressionAlgorithm
{
    public ContainerData Compress(ContainerData containerData)
    {
        using (var compressedStream = new MemoryStream())
        {
            using (var compressor = new DeflateStream(compressedStream, CompressionMode.Compress, leaveOpen: true))
            {
                containerData.Content.Position = 0;
                containerData.Content.CopyTo(compressor);
                compressor.Close();
                var file = new ContainerData(compressedStream);
                return file;
            }
        }
    }

    public ContainerData Decompress(ContainerData containerData)
    {
        containerData.Content.Position = 0;
        using (var deCompressedStream = new MemoryStream())
        {
            using (var compressor = new DeflateStream(containerData.Content, CompressionMode.Decompress))
            {
                compressor.CopyTo(deCompressedStream);
                deCompressedStream.Position = 0;
                return new ContainerData(deCompressedStream);
            }
        }
    }
}