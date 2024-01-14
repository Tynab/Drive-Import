namespace DriveImport.Common.Responses;

public sealed class DriveDirectoryResponse
{
    public string? Id { get; set; }

    public IEnumerable<Google.Apis.Drive.v3.Data.File>? Files { get; set; }
}
