using FreeBox.Server.Core.Entities;
using FreeBox.Server.Core.Interfaces;
using File = FreeBox.Server.Core.Entities.File;
using FileInfo = FreeBox.Server.Core.Entities.FileInfo;

namespace FreeBox.Server.Core.Repositories;

public class LocalFsRepository : IRepository
{
    private string _path;
    private ILogger<LocalFsRepository> _logger;
    private ICompressionAlgorithm _compressionAlgorithm;

    public LocalFsRepository(string path, ILogger<LocalFsRepository> logger, ICompressionAlgorithm compressionAlgorithm)
    {
        _path = path;
        _logger = logger;
        _compressionAlgorithm = compressionAlgorithm;
    }

    public FileStorage Save(File file)
    {
        string fileName = CreateUniqueFile();
        string filePath = Path.Combine(_path, fileName);
        using var stream = System.IO.File.OpenWrite(filePath);
        stream.Position = 0;
        using Stream compressedFile = _compressionAlgorithm.Compress(file.Content);
        compressedFile.Position = 0;
        compressedFile.CopyTo(stream);
        _logger.Log(LogLevel.Information, $"Created file: {filePath}, size: {stream.Length}");
        return new FileStorage(
            file.FileInfo,
            new FileInfo(fileName, compressedFile.Length, DateTime.Now));
    }

    public File GetFile(FileStorage file)
    {
        string fileName = file.Storage.Name;
        string filePath = Path.Combine(_path, fileName);
        if (!System.IO.File.Exists(filePath))
        {
            _logger.Log(LogLevel.Error, $"File {filePath} does not exists");
            throw new FileNotFoundException(filePath);
        }

        using Stream content = _compressionAlgorithm.Decompress(System.IO.File.OpenRead(filePath));
        var res = new File(file.File, content);
        content.Dispose();
        return res;
    }

    public void DeleteFile(FileStorage file)
    {
        string fileName = file.Storage.Name;
        string filePath = Path.Combine(_path, fileName);
        if (!System.IO.File.Exists(filePath))
        {
            _logger.Log(LogLevel.Error, $"File {filePath} does not exists");
            throw new FileNotFoundException(filePath);
        }
        System.IO.File.Delete(filePath);
        _logger.Log(LogLevel.Information, $"File {filePath} successfully deleted");
    }

    private string CreateUniqueFile()
    {
        string name;
        lock (this)
        {
            name = Guid.NewGuid().ToString();
            while (System.IO.File.Exists(Path.Combine(_path, name)))
            {
                name = Guid.NewGuid().ToString();
            }

            using (System.IO.File.Create(Path.Combine(_path, name)))
            {
            }
        }
        return name;
    }
}