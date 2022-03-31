using FreeBox.Server.Domain.Entities;

namespace FreeBox.Server.Core.Interfaces;

public interface ICompressionAlgorithm
{
    ContainerData Compress(ContainerData containerData);
    ContainerData Decompress(ContainerData containerData);
}