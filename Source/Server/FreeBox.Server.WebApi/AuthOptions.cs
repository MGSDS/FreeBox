﻿using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace FreeBox.Server.WebApi;

public class AuthOptions
{
    public const string ISSUER = "MyAuthServer";
    public const string AUDIENCE = "MyAuthClient";
    public const string KEY = "mysupersecret_secretkey!123";
    public const int LIFETIME = 1;
    public static SymmetricSecurityKey GetSymmetricSecurityKey()
    {
        return new SymmetricSecurityKey(Encoding.ASCII.GetBytes(KEY));
    }
}