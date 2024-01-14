namespace DriveImport.Layout;

public sealed partial class NavMenu
{
    private bool collapseNavMenu = true;

    private string? NavMenuCssClass => collapseNavMenu ? "collapse" : null;

    private void ToggleNavMenu() => collapseNavMenu = !collapseNavMenu;
}
