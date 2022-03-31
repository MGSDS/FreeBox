namespace FreeBox.Server.Core.Exceptions;

public class FreeBoxException : Exception
{
    public FreeBoxException()
    {
    }

    public FreeBoxException(string message)
        : base(message)
    {
    }

    public FreeBoxException(string message, Exception inner)
        : base(message, inner)
    {
    }
}