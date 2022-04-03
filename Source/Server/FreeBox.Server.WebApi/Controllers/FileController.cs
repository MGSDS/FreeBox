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
    private ILogger<FileController> _logger;

    public FileController(IFileService fileService, ILogger<FileController> logger)
    {
        _fileService = fileService;
        _logger = logger;
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
        _logger.LogInformation($"User {User.Identity!.Name} successfully uploaded file {fileInfo.Name}");
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
        {
            _logger.LogWarning($"User {User.Identity!.Name} unsuccessfully tried to get files of user {login}");
            return Forbid();
        }

        var files = _fileService
            .FindUserFiles(User.Identity!.Name!)
            .Select(x => x.ToDto())
            .ToList();

        _logger.LogInformation($"User {User.Identity!.Name} successfully gets files of user {login}");
        return files;
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
            _logger.LogWarning($"User {User.Identity!.Name} tried to delete non-existent file");
            return BadRequest("No such file found");
        }

        _logger.LogInformation($"User {User.Identity!.Name} successfully deleted file {containerInfoId}");
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
            _logger.LogWarning($"User {User.Identity!.Name} tried to download non-existent file ");
            return BadRequest("No such file found");
        }

        file.Data.Content.Position = 0;
        var result = new FileStreamResult(file.Data.Content, "application/octet-stream")
        {
            FileDownloadName = file.Info.Name,
        };

        _logger.LogInformation($"User {User.Identity!.Name} successfully downloaded file {containerInfoId}");
        return result;
    }
}