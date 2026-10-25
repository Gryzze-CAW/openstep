using System.Globalization;
using System.Resources;

namespace OpenSteps.Core.Services;

/// <summary>
/// Looks up user-facing texts in the Windows display language (Dutch or English, English as fallback).
/// </summary>
public static class Loc
{
    private static readonly ResourceManager Resources = new("OpenSteps.Core.Resources.Strings", typeof(Loc).Assembly);

    public static string LanguageCode => CultureInfo.CurrentUICulture.TwoLetterISOLanguageName == "nl" ? "nl" : "en";

    public static string Get(string key)
    {
        return Resources.GetString(key, CultureInfo.CurrentUICulture) ?? key;
    }

    public static string Format(string key, params object[] args)
    {
        return string.Format(CultureInfo.CurrentUICulture, Get(key), args);
    }
}
