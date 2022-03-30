using FreeBox.Shared.Dtos;

namespace FreeBox.Server.Core.Models;

public class User
{
    public User(Guid id, string name)
    {
        Id = id;
        Name = name;
    }

    public UserDto ToDto()
    {
        return new UserDto(Id, Name);
    }

    public Guid Id { get; }
    public string Name { get; }
}