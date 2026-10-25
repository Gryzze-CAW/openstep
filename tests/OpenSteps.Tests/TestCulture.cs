using System.Globalization;
using System.Runtime.CompilerServices;

namespace OpenSteps.Tests;

internal static class TestCulture
{
    // The existing tests assert English texts, whatever the display language of the PC that runs them.
    [ModuleInitializer]
    internal static void UseEnglish()
    {
        CultureInfo.DefaultThreadCurrentUICulture = CultureInfo.GetCultureInfo("en");
    }
}
