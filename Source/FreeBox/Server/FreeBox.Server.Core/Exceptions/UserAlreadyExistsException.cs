namespace FreeBox.Server.Core.Exceptions;

public class UserAlreadyExistsException : FreeBoxException
{
    public UserAlreadyExistsException()
    {
    }

    public UserAlreadyExistsException(string message)
        : base(message)
    {
    }

    public UserAlreadyExistsException(string message, Exception inner)
        : base(message, inner)
    {
    }
}