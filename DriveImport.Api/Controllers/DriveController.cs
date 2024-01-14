using Google.Apis.Auth.AspNetCore3;
using Google.Apis.Drive.v3;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using YANLib;
using static Google.Apis.Drive.v3.DriveService.ScopeConstants;
using static Google.Apis.Drive.v3.FilesResource;
using static Google.Apis.Services.BaseClientService;
using static System.IO.Path;
using static System.Linq.Enumerable;

namespace DriveImport.Api.Controllers;

[ApiController]
[Route("api/google-drive")]
public sealed class DriveController(ILogger<DriveController> logger) : ControllerBase
{
    private readonly ILogger<DriveController> _logger = logger;

    [HttpGet("items")]
    [GoogleScopedAuthorize(DriveReadonly)]
    public async ValueTask<IActionResult> GetAllItems([FromServices] IGoogleAuthProvider auth, string? directoryId = null)
    {
        try
        {
            if (directoryId.IsNull())
            {
                directoryId = "root";
            }

            var req = new DriveService(new Initializer
            {
                HttpClientInitializer = await auth.GetCredentialAsync()
            }).Files.List();

            req.Q = $"parents in '{directoryId}'";

            return Ok((await req.ExecuteAsync()).Files.OrderBy(x => x.Name));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "GetAllItemsDriveController-Exception: {DirectoryId}", directoryId);

            return Problem();
        }
    }

    [HttpDelete("items/{id}")]
    [GoogleScopedAuthorize(DriveReadonly)]
    public async ValueTask<IActionResult> DeleteItem([FromServices] IGoogleAuthProvider auth, string id)
    {
        try
        {
            return Ok(await new DriveService(new Initializer
            {
                HttpClientInitializer = await auth.GetCredentialAsync()
            }).Files.Delete(id).ExecuteAsync());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "DeleteItemDriveController-Exception");

            return Problem();
        }
    }

    [HttpPost("folders")]
    [GoogleScopedAuthorize(Drive)]
    public async ValueTask<IActionResult> CreateFolders([FromServices] IGoogleAuthProvider auth, [Required] string directoryName, string? directoryId = null)
    {
        try
        {
            if (directoryId.IsNull())
            {
                directoryId = "root";
            }

            var req = new DriveService(new Initializer
            {
                HttpClientInitializer = await auth.GetCredentialAsync()
            }).Files.Create(new Google.Apis.Drive.v3.Data.File()
            {
                Name = directoryName,
                MimeType = "application/vnd.google-apps.folder",
                Parents = new List<string>
                {
                    directoryId
                }
            });

            req.Fields = "id";

            return Ok((await req.ExecuteAsync()).Id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "CreateFoldersDriveController-Exception: {DirectoryId} - {DirectoryName}", directoryId, directoryName);

            return Problem();
        }
    }

    [HttpPost("upload-sheets")]
    [GoogleScopedAuthorize(Drive)]
    public async ValueTask<IActionResult> UploadSheets([FromServices] IGoogleAuthProvider auth, [Required] string filePath, string? directoryId = null)
    {
        try
        {
            var sprdShtExt = new string[]
            {
                ".xls", ".xlsx", ".csv", ".xlsm", ".xlsb", ".xltx", ".xltm", ".xlam", ".xlt", ".ods", ".gnumeric", ".numbers"
            };

            var ext = GetExtension(filePath);

            if (ext.IsWhiteSpaceOrNull() || !sprdShtExt.ToLowerInvariant().Contains(ext))
            {
                return BadRequest("The provided file format is not supported. Please upload a spreadsheet file.");
            }

            if (directoryId.IsNull())
            {
                directoryId = "root";
            }

            CreateMediaUpload req;

            using (var stream = new FileStream(filePath, FileMode.Open))
            {
                req = new DriveService(new Initializer
                {
                    HttpClientInitializer = await auth.GetCredentialAsync()
                }).Files.Create(new Google.Apis.Drive.v3.Data.File()
                {
                    Name = GetFileNameWithoutExtension(filePath),
                    MimeType = "application/vnd.google-apps.spreadsheet",
                    Parents = new List<string>
                    {
                        directoryId
                    }
                }, stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");

                req.Fields = "id";
                _ = await req.UploadAsync();
            }

            return Ok(req.ResponseBody.Id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "UploadSheetsDriveController-Exception: {FilePath} - {DirectoryId}", filePath, directoryId);

            return Problem();
        }
    }

    [HttpPost("logout")]
    public async ValueTask<IActionResult> Logout()
    {
        try
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            return Ok("Logged out successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "LogoutDriveController-Exception");

            return Problem();
        }
    }
}
