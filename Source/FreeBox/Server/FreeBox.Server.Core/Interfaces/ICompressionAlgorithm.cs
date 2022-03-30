using System.IO.Compression;

namespace FreeBox.Server.Core.Interfaces;

public interface ICompressionAlgorithm
{
    Stream Compress(Stream data);
    Stream Decompress(Stream data);
}