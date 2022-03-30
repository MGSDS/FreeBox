namespace FreeBox.Server.Core.Models;

public record FileInfo(Guid Id, string Name, long Size, DateTime SaveDate);