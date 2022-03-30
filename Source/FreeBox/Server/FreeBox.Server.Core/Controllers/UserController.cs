using FreeBox.Server.Core.Interfaces;
using FreeBox.Shared.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FreeBox.Server.Core.Controllers;

[ApiController]
[Route("/api/[controller]")]
public class UserController : ControllerBase
{
    private ILogger<UserController> _logger;
    private IUserService _userService;
    private IFileService _fileService;

    public UserController(ILogger<UserController> logger, IUserService userService, IFileService fileService)
    {
        _logger = logger;
        _userService = userService;
        _fileService = fileService;
    }

    [HttpPost]
    [Route("get/current")]
    [Authorize]
    public ActionResult<UserDto> GetUser()
    {
        var user = _userService.GetUser(User.Identity.Name);
        return user.ToDto();
    }
    
    [HttpDelete]
    [Route("delete")]
    [Authorize]
    public ActionResult DeleteUser()
    {
        _userService.DeleteUser(User.Identity.Name);
        
        return Ok();
    }

    [HttpPost]
    [Route("get/all")]
    [Authorize(Roles = "admin")]
    public ActionResult<List<UserDto>> GetUsers()
    {
        var user = _userService.GetUsers();
        return user.Select(x => x.ToDto()).ToList();
    }


}