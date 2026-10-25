using System.Windows.Markup;
using OpenSteps.Core.Services;

namespace OpenSteps.App;

/// <summary>
/// XAML markup extension for translated texts: Text="{loc:T Common_Cancel}".
/// </summary>
[MarkupExtensionReturnType(typeof(string))]
public sealed class TExtension : MarkupExtension
{
    public TExtension(string key)
    {
        Key = key;
    }

    public string Key { get; set; }

    public override object ProvideValue(IServiceProvider serviceProvider)
    {
        return Loc.Get(Key);
    }
}
