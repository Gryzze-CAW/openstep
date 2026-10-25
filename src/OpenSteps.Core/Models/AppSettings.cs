namespace OpenSteps.Core.Models;

public sealed class AppSettings
{
    public ScreenshotMode ScreenshotMode { get; set; } = ScreenshotMode.FullDesktop;

    /// <summary>"en" or "nl". Null follows the Windows display language.</summary>
    public string? Language { get; set; }
}
