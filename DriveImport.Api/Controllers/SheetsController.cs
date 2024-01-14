using Google.Apis.Auth.AspNetCore3;
using Google.Apis.Sheets.v4;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using static DriveImport.Api.Mappers.SheetsContentMapper;
using static Google.Apis.Services.BaseClientService;
using static System.Linq.Enumerable;

namespace DriveImport.Api.Controllers;

[ApiController]
[Route("api/google-sheet")]
public sealed class SheetsController(ILogger<SheetsController> logger) : ControllerBase
{
    private readonly ILogger<SheetsController> _logger = logger;

    [HttpGet("{range}")]
    [GoogleScopedAuthorize]
    public async ValueTask<IActionResult> Get([FromServices] IGoogleAuthProvider auth, [Required] string sheetsId, [Required] string sheetName, string range)
    {
        try
        {
            return Ok(MapFromRangeData((await new SheetsService(new Initializer
            {
                HttpClientInitializer = await auth.GetCredentialAsync()
            }).Spreadsheets.Values.Get(sheetsId, $"{sheetName}!{range}").ExecuteAsync()).Values).FirstOrDefault());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "GetSheetController-Exception: {SheetsId} - {SheetName} - {Row}", sheetsId, sheetName, range);

            return Problem();
        }
    }
}
