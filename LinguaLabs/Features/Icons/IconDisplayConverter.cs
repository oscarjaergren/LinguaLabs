using Microsoft.UI.Xaml.Data;

namespace LinguaLabs.Features.Icons;

public class IconDisplayConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        if (value is string iconName)
        {
            return GetIconDisplayText(iconName);
        }
        
        return "??";
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language)
    {
        throw new NotImplementedException();
    }

    private static string GetIconDisplayText(string iconName)
    {
        // Use first few characters of the name
        var name = iconName.Replace("-", "").Replace("_", "");
        if (name.Length >= 2)
            return name.Substring(0, Math.Min(2, name.Length)).ToUpper();
        
        // Fallback
        return iconName.Length >= 2 ? iconName.Substring(0, 2).ToUpper() : "??";
    }
}
