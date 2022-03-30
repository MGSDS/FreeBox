using FreeBox.Server.Core.Interfaces;
using FreeBox.Server.Core.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using File = FreeBox.Server.Core.Models.File;
using FileInfo = FreeBox.Server.Core.Models.FileInfo;

namespace FreeBox.Server.Core.Controllers;

[ApiController]
[Route("/api/[controller]")]
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
    [Route("upload")]
    [Authorize]
    public ActionResult<FileInfo> AddUserFile(IFormFile fileForm)
    {
        //TODO: Get User From Auth
        using Stream content = fileForm.OpenReadStream();
        var file = new File(new FileInfo(Guid.Empty, fileForm.FileName, content.Length, DateTime.Now), fileForm.OpenReadStream());
        FileInfo fileInfo = _fileService.SaveFile(file, User.Identity.Name);
        file.Dispose();
        return fileInfo;
    }

    
    [HttpGet]
    [Route("get/all")]
    [Authorize]
    public List<FileInfo> GetUserFiles()
    {
        return _fileService.GetUserFiles(User.Identity.Name);
    }
    
    [HttpDelete]
    [Route("delete/{id}")]
    [Authorize]
    public ActionResult DeleteFile([FromRoute] Guid id)
    {
        //TODO: forbid delete not yours
        _fileService.DeleteFile(new FileInfo(id, String.Empty, 0, DateTime.Now));
        return Ok();
    }
    
    [HttpGet]
    [Route("get/{id}")]
    [Authorize]
    public FileStreamResult GetFile([FromRoute] Guid id)
    {
        //TODO: forbid get not yours
        File file = _fileService.GetFile(new FileInfo(id, String.Empty, 0, DateTime.Now));
        file.Content.Position = 0;
        var result = new FileStreamResult(file.Content, "application/octet-stream");
        result.FileDownloadName = file.FileInfo.Name;
        return result;
    }
}