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

    public FileController(IFileService fileService)
    {
        _fileService = fileService;
    }
    
    [HttpPost]
    [Route("upload")]
    [Authorize]
    public ActionResult<FileInfo> AddUserFile(IFormFile fileForm)
    {
        using Stream content = fileForm.OpenReadStream();
        var file = new File(new FileInfo(Guid.Empty, fileForm.FileName, content.Length, DateTime.Now), fileForm.OpenReadStream());
        FileInfo fileInfo = _fileService.Save(file, User.Identity.Name);
        file.Dispose();
        return fileInfo;
    }

    
    [HttpGet]
    [Route("get/all")]
    [Authorize]
    public List<FileInfo> GetUserFiles()
    {
        return _fileService.Find(User.Identity.Name);
    }
    
    [HttpDelete]
    [Route("delete/{id}")]
    [Authorize]
    public ActionResult DeleteFile([FromRoute] Guid id)
    {
        _fileService.Delete(User.Identity.Name, id);
        return Ok();
    }
    
    [HttpGet]
    [Route("get/{id}")]
    [Authorize]
    public ActionResult<FileStreamResult> GetFile([FromRoute] Guid id)
    {
        File file = _fileService.Find(User.Identity.Name, id);

        file.Content.Position = 0;
        var result = new FileStreamResult(file.Content, "application/octet-stream");
        result.FileDownloadName = file.FileInfo.Name;
        return result;
    }
}