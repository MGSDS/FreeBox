using FreeBox.Server.Core.Interfaces;
using FreeBox.Server.Domain.Entities;
using FreeBox.Server.Utils.Extensions;
using FreeBox.Shared.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

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
    [Authorize(AuthenticationSchemes ="Bearer", Roles = "user")]
    public ActionResult<ContainerInfoDto> AddUserFile(IFormFile fileForm)
    {
        ContainerInfo info;
        ContainerData data;
        using (Stream content = fileForm.OpenReadStream())
        {
            data = new ContainerData(content);
            info = new ContainerInfo(fileForm.Name, fileForm.Length, DateTime.Now);
        }

        var file = new FileContainer(info, data);
        
        ContainerInfo fileInfo = _fileService.Save(file, User.Identity.Name);
        file.Dispose();
        return fileInfo.ToDto();
    }

    
    [HttpGet]
    [Route("get/all")]
    [Authorize(AuthenticationSchemes ="Bearer", Roles = "user")]
    public List<ContainerInfoDto> GetUserFiles()
    {
        return _fileService
            .Find(User.Identity.Name)
            .Select(x => x.ToDto())
            .ToList();
    }
    
    [HttpDelete]
    [Route("delete/{id}")]
    [Authorize(AuthenticationSchemes ="Bearer", Roles = "user")]
    public ActionResult DeleteFile([FromRoute] Guid id)
    {
        _fileService.Delete(User.Identity.Name, id);
        return Ok();
    }
    
    [HttpGet]
    [Route("get/{id}")]
    [Authorize(AuthenticationSchemes ="Bearer", Roles = "user")]
    public ActionResult<FileStreamResult> GetFile([FromRoute] Guid id)
    {
        var file = _fileService.Find(User.Identity.Name, id);
        file.Data.Content.Position = 0;
        var result = new FileStreamResult(file.Data.Content, "application/octet-stream");
        result.FileDownloadName = file.Info.Name;
        return result;
    }
}