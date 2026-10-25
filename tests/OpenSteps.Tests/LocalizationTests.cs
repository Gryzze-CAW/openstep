using System.Collections;
using System.Globalization;
using System.Resources;
using System.Text.RegularExpressions;
using OpenSteps.Core.Models;
using OpenSteps.Core.Services;

namespace OpenSteps.Tests;

public sealed class LocalizationTests
{
    private static readonly ResourceManager Resources = new("OpenSteps.Core.Resources.Strings", typeof(Loc).Assembly);

    [Fact]
    public void DutchAndEnglishHaveTheSameKeysAndPlaceholders()
    {
        var english = Load(CultureInfo.InvariantCulture);
        var dutch = Load(CultureInfo.GetCultureInfo("nl"));

        Assert.Equal(english.Keys.OrderBy(k => k), dutch.Keys.OrderBy(k => k));
        foreach (var key in english.Keys)
        {
            Assert.Equal(Placeholders(english[key]), Placeholders(dutch[key]));
        }
    }

    [Fact]
    public void Get_UsesDutchWhenTheDisplayLanguageIsDutch()
    {
        using var _ = new CultureScope("nl-BE");

        Assert.Equal("Annuleren", Loc.Get("Common_Cancel"));
        Assert.Equal("nl", Loc.LanguageCode);
    }

    [Fact]
    public void Get_FallsBackToEnglishForOtherLanguagesAndToTheKeyWhenMissing()
    {
        using var _ = new CultureScope("fr-BE");

        Assert.Equal("Cancel", Loc.Get("Common_Cancel"));
        Assert.Equal("No_Such_Key", Loc.Get("No_Such_Key"));
        Assert.Equal("en", Loc.LanguageCode);
    }

    [Fact]
    public void StepTitleGenerator_WritesDutchTitlesWithTheVerbInTheRightPlace()
    {
        using var _ = new CultureScope("nl-BE");
        var step = new RecordedStep
        {
            ActionType = StepActionType.DoubleClick,
            ElementName = "Opslaan",
            ControlType = "Button",
            UsefulElementFound = true
        };

        Assert.Equal("Dubbelklik op de knop 'Opslaan'", new StepTitleGenerator().Generate(step));

        step.ActionType = StepActionType.RightClick;
        Assert.Equal("Klik met de rechtermuisknop op de knop 'Opslaan'", new StepTitleGenerator().Generate(step));
    }

    [Fact]
    public void MarkdownBuilder_WritesDutchHeadings()
    {
        using var _ = new CultureScope("nl-BE");
        var session = new RecordingSession { Title = "Handleiding" };
        session.Steps.Add(new RecordedStep { GeneratedTitle = "Eerste" });

        var markdown = new MarkdownBuilder().BuildMarkdown(session);

        Assert.Contains("Gemaakt met OpenSteps.", markdown);
        Assert.Contains("## Stap 1: Eerste", markdown);
    }

    [Fact]
    public async Task SettingsService_KeepsTheLanguageChoice()
    {
        var path = Path.Combine(Path.GetTempPath(), "OpenSteps.Tests", Guid.NewGuid().ToString("N"), "settings.json");
        var service = new SettingsService(path);

        await service.SaveAsync(new AppSettings { Language = "nl" });

        Assert.Equal("nl", (await service.LoadAsync()).Language);
        Assert.Null((await new SettingsService(path + ".missing").LoadAsync()).Language);
    }

    private static Dictionary<string, string> Load(CultureInfo culture)
    {
        var set = Resources.GetResourceSet(culture, createIfNotExists: true, tryParents: false)!;
        return set.Cast<DictionaryEntry>().ToDictionary(e => (string)e.Key, e => (string)e.Value!);
    }

    private static string Placeholders(string value)
    {
        return string.Join(",", Regex.Matches(value, @"\{(\d+)").Select(m => m.Groups[1].Value).Distinct().OrderBy(v => v));
    }

    private sealed class CultureScope : IDisposable
    {
        private readonly CultureInfo _previous = CultureInfo.CurrentUICulture;

        public CultureScope(string name)
        {
            CultureInfo.CurrentUICulture = CultureInfo.GetCultureInfo(name);
        }

        public void Dispose()
        {
            CultureInfo.CurrentUICulture = _previous;
        }
    }
}
