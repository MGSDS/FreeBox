﻿namespace FreeBox.Server.Core.Exceptions;

public class UserNotFoundException : FreeBoxException
{
    public UserNotFoundException()
    {
    }

    public UserNotFoundException(string message)
        : base(message)
    {
    }

    public UserNotFoundException(string message, Exception inner)
        : base(message, inner)
    {
    }
}