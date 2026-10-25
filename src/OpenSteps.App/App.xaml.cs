using System.Configuration;
using System.Globalization;
using System.Data;
using System.Windows;
using OpenSteps.Core.Services;

namespace OpenSteps.App;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : System.Windows.Application
{
    protected override void OnStartup(StartupEventArgs e)
    {
        // The language must be known before the first window is created, because XAML texts are looked up while it loads.
        var language = Task.Run(() => new SettingsService().LoadAsync()).GetAwaiter().GetResult().Language;
        if (!string.IsNullOrWhiteSpace(language))
        {
            var culture = CultureInfo.GetCultureInfo(language);
            CultureInfo.DefaultThreadCurrentUICulture = culture;
            CultureInfo.CurrentUICulture = culture;
        }

        base.OnStartup(e);
    }
}

