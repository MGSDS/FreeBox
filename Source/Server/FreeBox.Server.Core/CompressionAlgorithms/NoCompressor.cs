using FreeBox.Server.Core.Interfaces;
using FreeBox.Server.Domain.Entities;

namespace FreeBox.Server.Core.CompressionAlgorithms;

public class NoCompressor : ICompressionAlgorithm
{
    public ContainerData Compress(ContainerData containerData)
    {
        return containerData;
    }

    public ContainerData Decompress(ContainerData containerData)
    {
        return containerData;
    }
}