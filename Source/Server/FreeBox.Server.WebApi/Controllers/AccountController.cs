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

namespace FreeBox.Server.WebApi.Controllers;

[ApiController]
[Route("/api/accounts")]
public class AccountController : Controller
{
    private readonly IUserService _userService;
    private ILogger<AccountController> _logger;

    public AccountController(IUserService userService, ILogger<AccountController> logger)
    {
        _userService = userService;
        _logger = logger;
    }

    [HttpPost]
    [Route("register")]
    public ActionResult<UserDto> CreateUser(UserCredentialsDto credentials)
    {
        if (string.IsNullOrEmpty(credentials.Login)
            || string.IsNullOrEmpty(credentials.Password)
            || string.IsNullOrWhiteSpace(credentials.Password)
            || string.IsNullOrWhiteSpace(credentials.Login))
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

        _logger.LogInformation($"User {credentials.Login} successfully registered");

        return user.ToDto();
    }

    [HttpPost]
    [Route("admin/register")]
    [Authorize(AuthenticationSchemes = "Bearer", Roles = "admin")]
    public ActionResult<UserDto> CreateAdminUser(UserCredentialsDto credentials)
    {
        _logger.LogInformation($"{HttpContext.Request.Path} invoked from {HttpContext.Connection.RemoteIpAddress?.ToString()}");
        if (string.IsNullOrEmpty(credentials.Login)
            || string.IsNullOrEmpty(credentials.Password)
            || string.IsNullOrWhiteSpace(credentials.Password)
            || string.IsNullOrWhiteSpace(credentials.Login))
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

        _logger.LogInformation($"Admin {credentials.Login} successfully registered by {User.Identity!.Name}");

        return user.ToDto();
    }

    [HttpPost]
    [Route("auth")]
    public ActionResult<AuthInfoDto> Token(UserCredentialsDto credentials)
    {
        if (string.IsNullOrEmpty(credentials.Login)
            || string.IsNullOrEmpty(credentials.Password)
            || string.IsNullOrWhiteSpace(credentials.Password)
            || string.IsNullOrWhiteSpace(credentials.Login))
            return BadRequest("login and password can not be empty");

        ClaimsIdentity? identity = GetIdentity(credentials.Login, credentials.Password);
        if (identity == null)
            return BadRequest("Invalid username or password.");

        DateTime now = DateTime.UtcNow;
        var jwt = new JwtSecurityToken(
            issuer: AuthOptions.ISSUER,
            audience: AuthOptions.AUDIENCE,
            notBefore: now,
            claims: identity.Claims,
            expires: now.Add(TimeSpan.FromMinutes(AuthOptions.LIFETIME)),
            signingCredentials: new SigningCredentials(
                AuthOptions.GetSymmetricSecurityKey(),
                SecurityAlgorithms.HmacSha256));
        string? encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);

        _logger.LogInformation($"User {User.Identity!.Name} successfully authenticated");

        return new AuthInfoDto(credentials.Login, encodedJwt);
    }

    [HttpDelete]
    [Route("delete/{login}")]
    [Authorize(AuthenticationSchemes ="Bearer", Roles = "user,admin")]
    public ActionResult DeleteUser(string login)
    {
        if (string.IsNullOrEmpty(login) || string.IsNullOrWhiteSpace(login))
            return BadRequest("login can not be empty");

        if (!User.IsInRole("admin") && login != User.Identity!.Name)
        {
            _logger.LogWarning($"User {User.Identity!.Name} tried to delete user {login}");
            return Forbid();
        }

        _userService.DeleteUser(User.Identity!.Name!);
        _logger.LogInformation($"User {User.Identity!.Name} successfully deleted user {login}");
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
            new Claim(ClaimsIdentity.DefaultRoleClaimType, user.Role),
        };
        ClaimsIdentity claimsIdentity = new(
            claims,
            "Token",
            ClaimsIdentity.DefaultNameClaimType,
            ClaimsIdentity.DefaultRoleClaimType);
        return claimsIdentity;
    }
}