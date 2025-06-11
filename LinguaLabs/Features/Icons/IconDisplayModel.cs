using System.Runtime.CompilerServices;

namespace LinguaLabs.Features.Icons;

public partial record IconDisplayModel
{
    public IconDisplayModel(IconifyIcon icon)
    {
        Icon = icon;
    }
    
    public IconifyIcon Icon { get; }

    public IFeed<string> DisplayText => Feed.Async<string>(ct => ValueTask.FromResult(GetIconDisplayText(Icon)));

    private static string GetIconDisplayText(IconifyIcon icon)
    {
        // Try to create a meaningful visual representation
        if (!string.IsNullOrEmpty(icon.Emoji))
            return icon.Emoji;

        // Use first few characters of the name
        var name = icon.Name.Replace("-", "").Replace("_", "");
        if (name.Length >= 2)
            return name[..Math.Min(2, name.Length)].ToUpper();

        // Fallback to icon set abbreviation
        return icon.Set.Length >= 2 ? icon.Set.Substring(0, 2).ToUpper() : "??";
    }
}
