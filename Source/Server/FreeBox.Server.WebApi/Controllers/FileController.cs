using FreeBox.Server.Core.Interfaces;
using FreeBox.Server.Domain.Entities;
using FreeBox.Server.Utils.Extensions;
using FreeBox.Shared.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FreeBox.Server.WebApi.Controllers;

[ApiController]
[Route("/api/files")]
public class FileController : ControllerBase
{
    private readonly IFileService _fileService;

    public FileController(IFileService fileService)
    {
        _fileService = fileService;
    }

    [HttpPost]
    [Route("upload")]
    [Authorize(AuthenticationSchemes ="Bearer", Roles = "user,admin")]
    public ActionResult<ContainerInfoDto> AddUserFile(IFormFile fileForm)
    {
        ContainerInfo info;
        ContainerData data;
        using (Stream content = fileForm.OpenReadStream())
        {
            data = new ContainerData(content);
            info = new ContainerInfo(fileForm.FileName, fileForm.Length, DateTime.Now);
        }

        using var file = new FileContainer(info, data);

        ContainerInfo fileInfo = _fileService.SaveFile(file, User.Identity!.Name!);
        file.Dispose();
        return fileInfo.ToDto();
    }

    [HttpGet]
    [Route("user/{login}/get/all")]
    [Authorize(AuthenticationSchemes ="Bearer", Roles = "user,admin")]
    public ActionResult<IEnumerable<ContainerInfoDto>> GetUserFiles(string login)
    {
        if (string.IsNullOrEmpty(login) || string.IsNullOrWhiteSpace(login))
            return BadRequest("login can not be empty");

        if (!User.IsInRole("admin") && login != User.Identity!.Name)
            return Forbid();
        return _fileService
            .FindUserFiles(User.Identity!.Name!)
            .Select(x => x.ToDto())
            .ToList();
    }

    [HttpDelete]
    [Route("delete/{containerInfoId}")]
    [Authorize(AuthenticationSchemes ="Bearer", Roles = "user,admin")]
    public ActionResult DeleteFile([FromRoute] Guid containerInfoId)
    {
        try
        {
            if (!User.IsInRole("admin") && _fileService.FindUserFiles(User.Identity!.Name!).All(x => x.Id != containerInfoId))
                return Forbid();
            _fileService.DeleteFile(containerInfoId);
        }
        catch (FileNotFoundException)
        {
            return BadRequest("No such file found");
        }

        return Ok();
    }

    [HttpGet]
    [Route("get/{containerInfoId}")]
    [Authorize(AuthenticationSchemes ="Bearer", Roles = "user,admin")]
    public IActionResult GetFile([FromRoute] Guid containerInfoId)
    {
        FileContainer file;
        try
        {
            if (!User.IsInRole("admin") && _fileService.FindUserFiles(User.Identity!.Name!).All(x => x.Id != containerInfoId))
                return Forbid();
            file = _fileService.GetFile(containerInfoId);
        }
        catch (FileNotFoundException)
        {
            return BadRequest("No such file found");
        }

        file.Data.Content.Position = 0;
        var result = new FileStreamResult(file.Data.Content, "application/octet-stream")
        {
            FileDownloadName = file.Info.Name,
        };
        return result;
    }
}