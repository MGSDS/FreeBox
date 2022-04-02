using System;
using System.IO;
using System.Linq;
using FreeBox.Server.Core.CompressionAlgorithms;
using FreeBox.Server.Core.Interfaces;
using FreeBox.Server.Domain.Entities;
using NUnit.Framework;

namespace FreeBox.Tests.ServerCoreTests;

public class ZipCompressorTests
{
    private ICompressionAlgorithm _compression;
    
    [SetUp]
    public void Setup()
    {
        _compression = new ZipCompressor();
    }

    [Test]
    public void CompressDecompress_DataEquals()
    {
        var rnd = new Random();
        var content = new byte[20];
        rnd.NextBytes(content);
        using var stream = new MemoryStream(content);
        using var testContainerData = new ContainerData(stream);

        var compressed = _compression.Compress(testContainerData);
        var decompressed = _compression.Decompress(compressed);
        
        Assert.True(testContainerData.Content.ToArray().SequenceEqual(decompressed.Content.ToArray()));
    }

}