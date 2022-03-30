using FreeBox.Server.Core.Entities;
using FreeBox.Server.Core.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using Microsoft.Net.Http.Headers;
using File = FreeBox.Server.Core.Entities.File;
using FileInfo = FreeBox.Server.Core.Entities.FileInfo;

namespace FreeBox.Server.Core.Controllers;

[ApiController]
[Route("/Api/[controller]")]
public class FileController : ControllerBase
{
    private IFileService _fileService;
    private IUserService _userService;

    public FileController(IFileService fileService, IUserService userService)
    {
        _fileService = fileService;
        _userService = userService;
    }

    [HttpPost]
    [Route("Add")]
    public ActionResult AddUserFile([FromQuery] Guid userId, IFormFile fileForm)
    {
        //TODO: Get User From Auth
        using Stream content = fileForm.OpenReadStream();
        var file = new File(new FileInfo(fileForm.FileName, content.Length, DateTime.Now), fileForm.OpenReadStream());
        _fileService.SaveFile(file, new User(userId, String.Empty));
        return Ok();
    }

    [HttpGet]
    [Route("/User/{id}")]
    public List<FileStorage> GetUserFiles([FromRoute] Guid id)
    {
        return _fileService.GetUserFiles(new User(id, String.Empty));
    }
    
    [HttpDelete]
    [Route("Delete")]
    public ActionResult DeleteFile([FromBody] FileStorage fileStorage)
    {
        _fileService.DeleteFile(fileStorage);
        return Ok();
    }
    
    [HttpGet]
    [Route("Get/{id}")]
    public FileStreamResult GetFile([FromRoute] Guid id)
    {
        File file = _fileService.GetFile(new FileStorage(null, null, id));
        file.Content.Position = 0;
        var result = new FileStreamResult(file.Content, "application/octet-stream");
        result.FileDownloadName = file.FileInfo.Name;
        return result;
    }
}