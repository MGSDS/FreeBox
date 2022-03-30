using File = FreeBox.Server.Core.Entities.File;

namespace FreeBox.Server.Core.Interfaces;

public interface ICompressionAlgorithm
{
    File Compress(File data);
    File Decompress(File data);
}