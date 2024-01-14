using DriveImport.Common.Responses;
using System.Net.Http.Json;

namespace DriveImport.Services.Implements;

public sealed class DriveService(ILogger<DriveService> logger, HttpClient httpClient) : IDriveService
{
    private readonly ILogger<DriveService> _logger = logger;
    private readonly HttpClient _httpClient = httpClient;

    public async ValueTask<DriveDirectoryResponse?> Get()
    {
        try
        {
            return await _httpClient.GetFromJsonAsync<DriveDirectoryResponse>("api/google-drive");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "GetAllDriveService-Exception");

            return default;
        }
    }
}
