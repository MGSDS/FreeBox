using File = FreeBox.Server.Core.Models.File;

namespace FreeBox.Server.Core.Interfaces;

public interface ICompressionAlgorithm
{
    File Compress(File data);
    File Decompress(File data);
}