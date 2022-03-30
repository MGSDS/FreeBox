using FreeBox.Server.Core.Interfaces;
using FreeBox.Shared.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace FreeBox.Server.Core.Controllers;

[ApiController]
[Route("/Api/[controller]")]
public class UserController : ControllerBase
{
    private ILogger<UserController> _logger;
    private IUserService _userService;

    public UserController(ILogger<UserController> logger, IUserService userService)
    {
        _logger = logger;
        _userService = userService;
        //TODO: log
    }

    [HttpPost]
    [Route("{name}/Create")]
    public ActionResult<UserDto> CreateUser([FromRoute] string name)
    {
        var user = _userService.AddUser(name);
        return user.ToDto();
    }

    [HttpPost]
    [Route("{id}")]
    public ActionResult<UserDto> GetUser([FromRoute] Guid id)
    {
        var user = _userService.GetUser(id);
        return user.ToDto();
    }
    
    [HttpDelete]
    [Route("{id}/Delete")]
    public ActionResult DeleteUser([FromRoute] Guid id)
    {
        _userService.DeleteUser(id);
        return Ok();
    }

    [HttpPost]
    [Route("GetAll")]
    public ActionResult<List<UserDto>> GetUsers()
    {
        var user = _userService.GetUsers();
        return user.Select(x => x.ToDto()).ToList();
    }


}