namespace FreeBox.Shared.Dtos;

public record FileInfoDto(string Name, long Size, DateTime SaveDate, Guid storageId)
{
    
}