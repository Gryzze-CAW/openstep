using OpenSteps.Core.Models;

namespace OpenSteps.Core.Services;

public static class ScreenshotModeLabels
{
    public static string GetDisplayText(ScreenshotMode mode)
    {
        return Loc.Get(mode == ScreenshotMode.ActiveWindow ? "Mode_ActiveWindow" : "Mode_FullDesktop");
    }
}
