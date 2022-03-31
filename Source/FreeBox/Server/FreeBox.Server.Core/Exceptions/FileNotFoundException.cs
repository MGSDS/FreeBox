namespace FreeBox.Server.Core.Exceptions;

public class FileNotFoundException : FreeBoxException
{
    public FileNotFoundException()
    {
    }

    public FileNotFoundException(string message)
        : base(message)
    {
    }

    public FileNotFoundException(string message, Exception inner)
        : base(message, inner)
    {
    }
}