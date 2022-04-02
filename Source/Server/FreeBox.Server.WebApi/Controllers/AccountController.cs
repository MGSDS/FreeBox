using System.IdentityModel.Tokens.Jwt;
using System.Security.Authentication;
using System.Security.Claims;
using FreeBox.Server.Core.Interfaces;
using FreeBox.Server.Domain.Entities;
using FreeBox.Server.Utils.Extensions;
using FreeBox.Shared.Dtos;
using FreeBox.Shared.Exceptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace FreeBox.Server.Core.Controllers;

[ApiController]
[Route("/api/accounts")]
public class AccountController : Controller
{
    private readonly IUserService _userService;

    public AccountController(IUserService userService)
    {
        _userService = userService;
    }
    
    [HttpPost]
    [Route("register")]
    public ActionResult<UserDto> CreateUser(UserCredentialsDto credentials)
    {
        if (String.IsNullOrEmpty(credentials.Login)
            || String.IsNullOrEmpty(credentials.Password)
            || String.IsNullOrWhiteSpace(credentials.Password)
            || String.IsNullOrWhiteSpace(credentials.Login))
            return BadRequest("login and password can not be empty");

        User user;
        try
        {
            user = _userService.AddUser(credentials.Login, credentials.Password, "user");
        }
        catch (UserAlreadyExistsException)
        {
            return BadRequest("User with such login already exists");
        }
        return user.ToDto();
    }

    [HttpPost]
    [Route("admin/register")]
    [Authorize(AuthenticationSchemes = "Bearer", Roles = "admin")]
    public ActionResult<UserDto> CreateAdminUser(UserCredentialsDto credentials)
    {
        if (String.IsNullOrEmpty(credentials.Login)
            || String.IsNullOrEmpty(credentials.Password) 
            || String.IsNullOrWhiteSpace(credentials.Password) 
            || String.IsNullOrWhiteSpace(credentials.Login))
            return BadRequest("login and password can not be empty");

        User user;
        try
        {
            user = _userService.AddUser(credentials.Login, credentials.Password, "admin");
        }
        catch (UserAlreadyExistsException)
        {
            return BadRequest("User with such login already exists");
        }
        return user.ToDto();
    }

    [HttpPost]
    [Route(("auth"))]
    public ActionResult<AuthInfoDto> Token(UserCredentialsDto credentials)
    {
        if (String.IsNullOrEmpty(credentials.Login)
            || String.IsNullOrEmpty(credentials.Password)
            || String.IsNullOrWhiteSpace(credentials.Password)
            || String.IsNullOrWhiteSpace(credentials.Login))
            return BadRequest("login and password can not be empty");

        var identity = GetIdentity(credentials.Login, credentials.Password);
        if (identity == null)
            return BadRequest("Invalid username or password.");

        var now = DateTime.UtcNow;
        var jwt = new JwtSecurityToken(
            issuer: AuthOptions.ISSUER,
            audience: AuthOptions.AUDIENCE,
            notBefore: now,
            claims: identity.Claims,
            expires: now.Add(TimeSpan.FromMinutes(AuthOptions.LIFETIME)),
            signingCredentials: new SigningCredentials(AuthOptions.GetSymmetricSecurityKey(),
                SecurityAlgorithms.HmacSha256));
        var encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);

        return new AuthInfoDto(credentials.Login, encodedJwt);
    }

    [HttpDelete]
    [Route("delete/{login}")]
    [Authorize(AuthenticationSchemes ="Bearer", Roles = "user,admin")]
    public ActionResult DeleteUser(string login)
    {
        if (String.IsNullOrEmpty(login) || String.IsNullOrWhiteSpace(login))
            return BadRequest("login can not be empty");

        if (!User.IsInRole("admin") && login != User.Identity.Name)
            return Forbid();

        _userService.DeleteUser(User.Identity.Name);
        return Ok();
    }
    
    private ClaimsIdentity? GetIdentity(string login, string password)
    {
        User user;
        try
        {
            user = _userService.FindUser(login);
            if (user.Password != password)
                return null;
        }
        catch (Exception e)
        {
            if (e is InvalidCredentialException or UserNotFoundException)
                return null;
            throw;
        }

        var claims = new List<Claim>
        {
            new Claim(ClaimsIdentity.DefaultNameClaimType, user.Login),
            new Claim(ClaimsIdentity.DefaultRoleClaimType, user.Role)
        };
        ClaimsIdentity claimsIdentity =
            new ClaimsIdentity(claims, "Token", ClaimsIdentity.DefaultNameClaimType,
                ClaimsIdentity.DefaultRoleClaimType);
        return claimsIdentity;
    }
}