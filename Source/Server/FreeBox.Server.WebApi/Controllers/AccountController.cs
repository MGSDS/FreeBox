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
[Route("/api/[controller]")]
public class AccountController : Controller
{
    private readonly IUserService _userService;

    public AccountController(IUserService userService)
    {
        _userService = userService;
    }
    
    [HttpPost]
    [Route("register")]
    public ActionResult<UserDto> CreateUser([FromForm] string login, [FromForm] string password)
    {
        User user;
        try
        {
            user = _userService.Add(login, password);
        }
        catch (UserAlreadyExistsException)
        {
            return BadRequest("User with such login already exists");
        }
        return user.ToDto();
    }

    [HttpPost]
    [Route(("login"))]
    public IActionResult Token(string username, string password)
    {
        var identity = GetIdentity(username, password);
        if (identity == null)
        {
            return BadRequest("Invalid username or password.");
        }

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

        var response = new
        {
            access_token = encodedJwt,
            username = identity.Name
        };

        return Json(response);
    }
    
    [HttpPost]
    [Route("get/current")]
    [Authorize(AuthenticationSchemes ="Bearer", Roles = "user")]
    public ActionResult<UserDto> GetUser()
    {
        var user = _userService.Find(User.Identity.Name);
        return user.ToDto();
    }
    
    [HttpDelete]
    [Route("delete")]
    [Authorize(AuthenticationSchemes ="Bearer", Roles = "user")]
    public ActionResult DeleteUser()
    {
        _userService.Delete(User.Identity.Name);
        return Ok();
    }
    
    private ClaimsIdentity? GetIdentity(string login, string password)
    {
        User user;
        try
        {
            user = _userService.Find(login, password);
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