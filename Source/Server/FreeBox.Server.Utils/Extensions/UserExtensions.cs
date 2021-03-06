using FreeBox.Server.Domain.Entities;
using FreeBox.Shared.Dtos;

namespace FreeBox.Server.Utils.Extensions;

public static class UserExtensions
{
    public static UserDto ToDto(this User user)
    {
        return new UserDto(user.Login, user.Role);
    }
}