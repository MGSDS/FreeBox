using FreeBox.Server.Core.Interfaces;
using FreeBox.Server.Core.Models;
using Microsoft.AspNetCore.Mvc;
using File = FreeBox.Server.Core.Models.File;
using FileInfo = FreeBox.Server.Core.Models.FileInfo;

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
    public ActionResult<FileInfo> AddUserFile([FromQuery] Guid userId, IFormFile fileForm)
    {
        //TODO: Get User From Auth
        using Stream content = fileForm.OpenReadStream();
        var file = new File(new FileInfo(Guid.Empty, fileForm.FileName, content.Length, DateTime.Now), fileForm.OpenReadStream());
        FileInfo fileInfo = _fileService.SaveFile(file, new User(userId, String.Empty));
        file.Dispose();
        return fileInfo;
    }

    [HttpGet]
    [Route("/User/{id}")]
    public List<FileInfo> GetUserFiles([FromRoute] Guid id)
    {
        return _fileService.GetUserFiles(new User(id, String.Empty));
    }
    
    [HttpDelete]
    [Route("Delete/{id}")]
    public ActionResult DeleteFile([FromRoute] Guid id)
    {
        _fileService.DeleteFile(new FileInfo(id, String.Empty, 0, DateTime.Now));
        return Ok();
    }
    
    [HttpGet]
    [Route("Get/{id}")]
    public FileStreamResult GetFile([FromRoute] Guid id)
    {
        File file = _fileService.GetFile(new FileInfo(id, String.Empty, 0, DateTime.Now));
        file.Content.Position = 0;
        var result = new FileStreamResult(file.Content, "application/octet-stream");
        result.FileDownloadName = file.FileInfo.Name;
        return result;
    }
}