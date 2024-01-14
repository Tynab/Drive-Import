using DriveImport.Common.Responses;

namespace DriveImport.Services;

public interface IDriveService
{
    public ValueTask<DriveDirectoryResponse?> Get();
}
