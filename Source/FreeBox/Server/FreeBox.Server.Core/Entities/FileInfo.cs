namespace FreeBox.Server.Core.Entities;

public record FileInfo(Guid Id, string Name, long Size, DateTime SaveDate);