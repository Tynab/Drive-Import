using DriveImport.Layout;

namespace DriveImport.Pages;

public sealed partial class ImportFilePage
{
    protected override async Task OnInitializedAsync()
    {
        try
        {
            Names = (await DriveService.Get())?.Files?.Select(x => x.Name);
        }
        catch (Exception ex)
        {
            Error?.ProcessError(ex);
        }
    }

    private Error? Error { get; set; }

    private IEnumerable<string>? Names { get; set; }
}
