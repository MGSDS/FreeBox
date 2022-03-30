using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using FreeBox.Server.Core.Extensions;
using FreeBox.Server.Core.Interfaces;
using FreeBox.Server.DataAccess;
using FreeBox.Shared.Dtos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace FreeBox.Server.Core.Controllers;

[ApiController]
[Route("/api/[controller]")]
public class AccountController : Controller
{
    private readonly FreeBoxContext _context;
    private readonly IUserService _userService;

    public AccountController(FreeBoxContext context, IUserService userService)
    {
        _context = context;
        _userService = userService;
        //TODO: move to accountService
    }
    
    [HttpPost]
    [Route("register")]
    public ActionResult<UserDto> CreateUser([FromForm] string login, [FromForm] string password)
    {
        var user = _userService.AddUser(login, password);
        return user.ToDto();
    }

    [HttpPost]
    [Route(("login"))]
    public IActionResult Token(string username, string password)
    {
        var identity = GetIdentity(username, password);
        if (identity == null)
        {
            return BadRequest(new {errorText = "Invalid username or password."});
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

    private ClaimsIdentity GetIdentity(string username, string password)
    {
        var user = _context.Users.FirstOrDefault(x => x.Login == username && x.Password == password)!.ToUser();
        if (user == null) return null;
        var claims = new List<Claim>
        {
            new Claim(ClaimsIdentity.DefaultNameClaimType, user.Login),
            new Claim(ClaimsIdentity.DefaultRoleClaimType, user.Role),
        };
        ClaimsIdentity claimsIdentity =
            new ClaimsIdentity(claims, "Token", ClaimsIdentity.DefaultNameClaimType,
                ClaimsIdentity.DefaultRoleClaimType);
        return claimsIdentity;
    }
}